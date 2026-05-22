# One-time per-machine setup: copy the repo's git hooks from .claude\git-hooks
# into .git\hooks. Idempotent; re-run safely to refresh.
#
# Why not git config core.hooksPath? Worktrees confuse it on Windows in some
# git versions. Simple file-copy works everywhere.

$ErrorActionPreference = 'Stop'

function Resolve-MainCheckoutPath {
    $repoToplevel = (git rev-parse --show-toplevel).Trim()
    $gitDir = (git rev-parse --git-common-dir).Trim()
    if (-not [System.IO.Path]::IsPathRooted($gitDir)) {
        $gitDir = Join-Path $repoToplevel $gitDir
    }
    return @{
        Repo = (Resolve-Path -LiteralPath (Split-Path -Parent $gitDir)).Path
        GitDir = (Resolve-Path -LiteralPath $gitDir).Path
    }
}

$paths = Resolve-MainCheckoutPath
$srcHooks = Join-Path $paths.Repo '.claude\git-hooks'
$dstHooks = Join-Path $paths.GitDir 'hooks'

if (-not (Test-Path -LiteralPath $srcHooks)) {
    throw "Source hooks dir not found: $srcHooks"
}
if (-not (Test-Path -LiteralPath $dstHooks)) {
    New-Item -ItemType Directory -Path $dstHooks -Force | Out-Null
}

$installed = 0
Get-ChildItem -LiteralPath $srcHooks -File | ForEach-Object {
    $dst = Join-Path $dstHooks $_.Name
    Copy-Item -LiteralPath $_.FullName -Destination $dst -Force
    Write-Output "installed: $($_.Name) -> $dst"
    $installed++
}

if ($installed -eq 0) {
    Write-Warning 'No hook files found in .claude\git-hooks.'
} else {
    Write-Output "$installed hook(s) installed."
}
