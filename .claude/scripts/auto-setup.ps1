# Self-healing per-machine setup. Runs on every Claude Code SessionStart and
# also stand-alone via install.bat. Idempotent — fast no-op when nothing needs
# to change.
#
# Does the following, only when each one isn't already in place:
#   1. Junctions ~\.claude\projects\<project-key>\memory to in-repo .claude\memory
#   2. Copies .claude\git-hooks\* into the repo's .git\hooks\
#   3. On first-run only (anything needed step 1), seeds NT's Indicators folder
#      via deploy-all.ps1 so a fresh-cloned machine compiles immediately.
#
# Never exits non-zero — failures are logged but do not block the session.
#
# Flags:
#   -Quiet    : no stdout unless we did something new this run
#   -Verbose  : extra per-step output

[CmdletBinding()]
param(
    [switch]$Quiet
)

$ErrorActionPreference = 'Stop'

$logFile = Join-Path $env:USERPROFILE '.claude\auto-setup.log'
function Write-Log {
    param([string]$Message, [string]$Level = 'INFO')
    try {
        $dir = Split-Path -Parent $logFile
        if (-not (Test-Path -LiteralPath $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
        Add-Content -LiteralPath $logFile -Value ("{0} {1} {2}" -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'), $Level, $Message)
    } catch {}
}

function Say {
    param([string]$Message)
    if (-not $Quiet) { Write-Output $Message }
}

try {
    # Locate the repo's main checkout (so we work the same in main or in a worktree).
    $repoToplevel = (git rev-parse --show-toplevel 2>$null)
    if ([string]::IsNullOrWhiteSpace($repoToplevel)) {
        Write-Log 'Not in a git repo; nothing to do.' 'SKIP'
        exit 0
    }
    $repoToplevel = $repoToplevel.Trim()
    $gitCommonDir = (git rev-parse --git-common-dir).Trim()
    if (-not [System.IO.Path]::IsPathRooted($gitCommonDir)) {
        $gitCommonDir = Join-Path $repoToplevel $gitCommonDir
    }
    $repoMain = (Resolve-Path -LiteralPath (Split-Path -Parent $gitCommonDir)).Path
    $gitCommonDir = (Resolve-Path -LiteralPath $gitCommonDir).Path

    # Memory junction state ------------------------------------------------
    $projectKey = ($repoMain -replace '[\\:/]', '-')
    $targetMemoryDir = Join-Path $env:USERPROFILE ".claude\projects\$projectKey\memory"
    $sourceMemoryDir = Join-Path $repoMain '.claude\memory'
    $junctionOk = $false
    if (Test-Path -LiteralPath $targetMemoryDir) {
        $item = Get-Item -LiteralPath $targetMemoryDir -Force
        $isReparse = ($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0
        if ($isReparse) {
            $currentTarget = $null
            try { $currentTarget = $item.Target } catch {}
            if ($currentTarget -and ($currentTarget -ieq $sourceMemoryDir)) {
                $junctionOk = $true
            }
        }
    }

    # Git hook state -------------------------------------------------------
    $hookSrcDir = Join-Path $repoMain '.claude\git-hooks'
    $hookDstDir = Join-Path $gitCommonDir 'hooks'
    $hookOk = $true
    if (Test-Path -LiteralPath $hookSrcDir) {
        Get-ChildItem -LiteralPath $hookSrcDir -File | ForEach-Object {
            $dst = Join-Path $hookDstDir $_.Name
            if (-not (Test-Path -LiteralPath $dst)) {
                $hookOk = $false
            } else {
                $srcHash = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash
                $dstHash = (Get-FileHash -LiteralPath $dst -Algorithm SHA256).Hash
                if ($srcHash -ne $dstHash) { $hookOk = $false }
            }
        }
    }

    # Worktree cleanup runs every session (independent of fast-path). It's a
    # fast no-op when nothing is stale and never touches the current worktree.
    $cleanupScript = Join-Path $repoMain '.claude\scripts\cleanup-worktrees.ps1'
    if (Test-Path -LiteralPath $cleanupScript) {
        & $cleanupScript -Quiet 2>&1 | ForEach-Object {
            Write-Log "cleanup-worktrees: $_"
            Write-Verbose $_
        }
    }

    # Fast path — nothing to do.
    if ($junctionOk -and $hookOk) {
        Write-Log 'noop'
        exit 0
    }

    Say '[auto-setup] First-run / repair detected. Setting up multi-machine sync...'
    $isFirstRun = $false

    if (-not $junctionOk) {
        Say '  -> linking memory'
        & (Join-Path $repoMain '.claude\scripts\link-memory.ps1') 2>&1 | ForEach-Object {
            Write-Log "link-memory: $_"
            Write-Verbose $_
        }
        Write-Log 'memory junction created/repaired' 'OK'
        $isFirstRun = $true
    }

    if (-not $hookOk) {
        Say '  -> installing git hooks'
        & (Join-Path $repoMain '.claude\scripts\install-git-hooks.ps1') 2>&1 | ForEach-Object {
            Write-Log "install-git-hooks: $_"
            Write-Verbose $_
        }
        Write-Log 'git hooks installed/repaired' 'OK'
    }

    # First-run only: seed NT's Indicators folder so a freshly-cloned machine
    # has everything it needs to compile without an extra step.
    if ($isFirstRun) {
        $deployScript = Join-Path $repoMain '.claude\scripts\deploy-all.ps1'
        $ntDir = Join-Path ([Environment]::GetFolderPath('MyDocuments')) 'NinjaTrader 8\bin\Custom\Indicators'
        if ((Test-Path -LiteralPath $deployScript) -and (Test-Path -LiteralPath $ntDir)) {
            Say '  -> seeding NinjaTrader Indicators folder (first run only)'
            & $deployScript -Quiet 2>&1 | ForEach-Object {
                Write-Log "deploy-all: $_"
                Write-Verbose $_
            }
            Write-Log 'first-run NT seed completed' 'OK'
        } else {
            Write-Log "skipped NT seed (deploy script or NT folder missing: $ntDir)" 'SKIP'
        }
    }

    Say '[auto-setup] Done.'
    exit 0
}
catch {
    Write-Log "ERROR: $($_.Exception.Message)`n$($_.ScriptStackTrace)" 'ERROR'
    Write-Warning "[auto-setup] $($_.Exception.Message) (logged to $logFile)"
    # Never fail the session — log and exit 0.
    exit 0
}
