# Self-healing worktree cleanup. Removes stale claude/* worktrees whose
# branches are fully merged to origin/main and have no uncommitted work.
# Never touches the main checkout or the current worktree.
#
# Designed to run from auto-setup.ps1 on SessionStart so cleanup operates on
# PREVIOUS sessions' worktrees, never the live one.
#
# Idempotent — fast no-op when there's nothing stale.
# Never exits non-zero — failures are logged but do not block the session.
#
# Flags:
#   -Quiet    : no stdout unless we removed something this run
#   -DryRun   : print what would be removed without doing it

[CmdletBinding()]
param(
    [switch]$Quiet,
    [switch]$DryRun
)

$ErrorActionPreference = 'Stop'

$logFile = Join-Path $env:USERPROFILE '.claude\auto-setup.log'
function Write-Log {
    param([string]$Message, [string]$Level = 'INFO')
    try {
        $dir = Split-Path -Parent $logFile
        if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
        Add-Content -LiteralPath $logFile -Value ("{0} {1} cleanup-worktrees: {2}" -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'), $Level, $Message)
    } catch {}
}

function Say {
    param([string]$Message)
    if (-not $Quiet) { Write-Output $Message }
}

try {
    $repoToplevel = (git rev-parse --show-toplevel 2>$null)
    if ([string]::IsNullOrWhiteSpace($repoToplevel)) {
        Write-Log 'not in a git repo; skipping' 'SKIP'
        exit 0
    }

    # Need origin/main as the merge yardstick. If absent (e.g. brand-new
    # clone before first fetch), bail rather than risk false-positives.
    git rev-parse --verify origin/main 2>$null | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Log 'origin/main not present; skipping' 'SKIP'
        exit 0
    }

    # Parse `git worktree list --porcelain`. Format per entry:
    #   worktree <path>
    #   HEAD <sha>
    #   branch <refs/heads/...>      (or `detached`)
    #   <blank line>
    $porcelain = git worktree list --porcelain
    $worktrees = @()
    $cur = $null
    foreach ($line in $porcelain) {
        if ($line -match '^worktree (.+)$') {
            $cur = [ordered]@{ path = $Matches[1]; branch = $null }
        } elseif ($line -match '^branch (.+)$' -and $cur) {
            $cur.branch = $Matches[1]
        } elseif ([string]::IsNullOrWhiteSpace($line) -and $cur) {
            $worktrees += [PSCustomObject]$cur
            $cur = $null
        }
    }
    if ($cur) { $worktrees += [PSCustomObject]$cur }

    # Determine current worktree path. Claude Code sets CLAUDE_PROJECT_DIR;
    # fall back to CWD when running stand-alone.
    $currentPath = $env:CLAUDE_PROJECT_DIR
    if ([string]::IsNullOrWhiteSpace($currentPath)) { $currentPath = (Get-Location).Path }
    try { $currentPath = (Resolve-Path -LiteralPath $currentPath).Path } catch {}

    $removed = 0
    $kept    = 0
    foreach ($wt in $worktrees) {
        $wtPath = $null
        try { $wtPath = (Resolve-Path -LiteralPath $wt.path -ErrorAction Stop).Path } catch { continue }

        # Only consider claude/* branches. Skips main checkout (no branch in
        # the claude/ namespace) and any user-owned branches.
        if (-not $wt.branch -or $wt.branch -notmatch '^refs/heads/claude/') { continue }

        # Never remove the worktree this script is running from.
        if ($wtPath -ieq $currentPath) { continue }

        $branchName = $wt.branch -replace '^refs/heads/', ''

        # Skip if there's uncommitted work — never lose user changes.
        $dirty = git -C $wtPath status --porcelain
        if (-not [string]::IsNullOrWhiteSpace(($dirty -join "`n"))) {
            Write-Log "keep (uncommitted): $wtPath"
            $kept++
            continue
        }

        # Skip if the branch has commits not yet on origin/main.
        $ahead = git rev-list --count "origin/main..$branchName" 2>$null
        if ($LASTEXITCODE -ne 0) {
            Write-Log "keep (rev-list failed): $branchName"
            $kept++
            continue
        }
        if ([int]$ahead -gt 0) {
            Write-Log "keep (unmerged, $ahead commits ahead of origin/main): $branchName"
            $kept++
            continue
        }

        if ($DryRun) {
            Say "[cleanup-worktrees] WOULD remove $wtPath ($branchName)"
            Write-Log "dry-run would remove $wtPath ($branchName)"
            continue
        }

        Say "[cleanup-worktrees] removing $wtPath ($branchName)"
        git worktree remove --force $wtPath 2>&1 | ForEach-Object { Write-Log "  remove: $_" }
        if ($LASTEXITCODE -eq 0) {
            git branch -D $branchName 2>&1 | ForEach-Object { Write-Log "  branch -D: $_" }
            $removed++
        } else {
            Write-Log "worktree remove failed for $wtPath (exit $LASTEXITCODE)" 'WARN'
        }
    }

    # Drop any zombie entries (directory manually deleted, git still tracks).
    git worktree prune 2>&1 | ForEach-Object { Write-Log "  prune: $_" }

    if ($removed -eq 0 -and -not $DryRun) {
        Write-Log "noop (kept=$kept)"
    } elseif ($removed -gt 0) {
        Write-Log "removed $removed, kept $kept" 'OK'
    }
    exit 0
}
catch {
    Write-Log "ERROR: $($_.Exception.Message)`n$($_.ScriptStackTrace)" 'ERROR'
    Write-Warning "[cleanup-worktrees] $($_.Exception.Message) (logged to $logFile)"
    exit 0
}
