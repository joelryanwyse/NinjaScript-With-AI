# One-time per-machine setup: junction the in-repo .claude\memory directory
# into Claude Code's standard project memory path so memory rides on git.
#
# Run from the repo root:
#   .\.claude\scripts\link-memory.ps1
#
# What it does:
#   1. Computes Claude Code's project key from the repo's main checkout path.
#      Key format: drive+path with separators flattened to '-' (e.g. C--Users-foo-bar).
#   2. Target = ~\.claude\projects\<key>\memory
#   3. If target already exists as a real directory, moves it to a timestamped
#      backup so nothing is lost.
#   4. Creates a directory junction: target -> <repo>\.claude\memory.
#
# Junctions don't need admin rights and survive across reboots. Re-running the
# script is safe — if the junction already points at the right place, it exits
# 0 with no changes.

$ErrorActionPreference = 'Stop'

function Resolve-MainCheckoutPath {
    # If we're inside a git worktree, the .git file points at the real .git
    # dir under the main checkout — strip back from there to find the main
    # working tree.
    $repoToplevel = (git rev-parse --show-toplevel).Trim()
    $gitDir = (git rev-parse --git-common-dir).Trim()
    if (-not [System.IO.Path]::IsPathRooted($gitDir)) {
        $gitDir = Join-Path $repoToplevel $gitDir
    }
    # main checkout = parent of common .git directory (which is <main>\.git)
    $mainCheckout = Split-Path -Parent $gitDir
    return (Resolve-Path -LiteralPath $mainCheckout).Path
}

function ConvertTo-ClaudeProjectKey {
    param([string]$Path)
    # Claude Code encodes the path by replacing '\', ':', '/' with '-'.
    # e.g. C:\Users\joelw\source\NinjaScript-With-AI ->
    #      C--Users-joelw-source-NinjaScript-With-AI
    return ($Path -replace '[\\:/]', '-')
}

$repoMain = Resolve-MainCheckoutPath
$projectKey = ConvertTo-ClaudeProjectKey -Path $repoMain
$claudeProjectsRoot = Join-Path $env:USERPROFILE '.claude\projects'
$targetMemoryDir = Join-Path $claudeProjectsRoot "$projectKey\memory"
$sourceMemoryDir = Join-Path $repoMain '.claude\memory'

Write-Output "Repo main checkout : $repoMain"
Write-Output "Project key        : $projectKey"
Write-Output "In-repo memory     : $sourceMemoryDir"
Write-Output "Claude memory path : $targetMemoryDir"

if (-not (Test-Path -LiteralPath $sourceMemoryDir)) {
    throw "In-repo memory dir not found at $sourceMemoryDir. Did you pull the latest main?"
}

# Ensure parent project dir exists.
$projectDir = Split-Path -Parent $targetMemoryDir
if (-not (Test-Path -LiteralPath $projectDir)) {
    New-Item -ItemType Directory -Path $projectDir -Force | Out-Null
}

# If target exists, figure out what it is.
if (Test-Path -LiteralPath $targetMemoryDir) {
    $item = Get-Item -LiteralPath $targetMemoryDir -Force
    $isReparse = ($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0
    if ($isReparse) {
        # Already a junction or symlink — check if it points where we want.
        $currentTarget = $null
        try { $currentTarget = (Get-Item -LiteralPath $targetMemoryDir -Force).Target } catch {}
        if ($currentTarget -and ($currentTarget -ieq $sourceMemoryDir)) {
            Write-Output 'Junction already points at in-repo memory. Nothing to do.'
            exit 0
        }
        Write-Output "Existing junction points at '$currentTarget' — removing."
        cmd /c rmdir "`"$targetMemoryDir`"" | Out-Null
    } else {
        # Real directory — back it up so we don't lose anything.
        $ts = Get-Date -Format 'yyyyMMdd-HHmmss'
        $backup = Join-Path $projectDir "memory.pre-junction-$ts"
        Write-Output "Existing memory directory found — backing up to $backup"
        Move-Item -LiteralPath $targetMemoryDir -Destination $backup
    }
}

# Create the junction (mklink /J needs cmd; works without admin for directories).
$null = cmd /c mklink /J "`"$targetMemoryDir`"" "`"$sourceMemoryDir`""
if ($LASTEXITCODE -ne 0) {
    throw "mklink /J failed (exit $LASTEXITCODE)"
}

Write-Output 'Junction created.'
Write-Output 'Verify with: dir "{0}"' -f $targetMemoryDir
