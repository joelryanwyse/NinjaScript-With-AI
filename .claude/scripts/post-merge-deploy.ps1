# Called from the post-merge git hook. Reads $env:NT_DEPLOY_FILES (newline-
# separated list of repo-root-relative paths) and deploys exactly those files
# to NT's Indicators folder.
#
# Run independently of the hook with:
#   $env:NT_DEPLOY_FILES = "aiECT.cs`naiOther.cs"
#   .\.claude\scripts\post-merge-deploy.ps1

$ErrorActionPreference = 'Stop'

function Resolve-MainCheckoutPath {
    $repoToplevel = (git rev-parse --show-toplevel).Trim()
    $gitDir = (git rev-parse --git-common-dir).Trim()
    if (-not [System.IO.Path]::IsPathRooted($gitDir)) {
        $gitDir = Join-Path $repoToplevel $gitDir
    }
    return (Resolve-Path -LiteralPath (Split-Path -Parent $gitDir)).Path
}

$srcDir = Resolve-MainCheckoutPath
$dstDir = Join-Path ([Environment]::GetFolderPath('MyDocuments')) 'NinjaTrader 8\bin\Custom\Indicators'

if (-not (Test-Path -LiteralPath $dstDir)) {
    Write-Warning "NT Indicators folder not found at $dstDir; skipping post-merge deploy."
    exit 0
}

$rawList = $env:NT_DEPLOY_FILES
if ([string]::IsNullOrWhiteSpace($rawList)) {
    Write-Output 'No files passed via NT_DEPLOY_FILES; nothing to do.'
    exit 0
}

$files = $rawList -split "`r?`n" | Where-Object { $_ -and ($_ -like '*.cs') }
$copied = 0
$errors = 0

foreach ($rel in $files) {
    $src = Join-Path $srcDir $rel
    if (-not (Test-Path -LiteralPath $src)) {
        Write-Warning "Source missing (deleted by merge?): $rel"
        continue
    }
    $dst = Join-Path $dstDir (Split-Path -Leaf $rel)
    try {
        Copy-Item -LiteralPath $src -Destination $dst -Force
        $copied++
        Write-Output "[post-merge] deployed $rel"
    } catch {
        $errors++
        Write-Warning "[post-merge] FAILED $rel : $($_.Exception.Message)"
    }
}

Write-Output "[post-merge] deployed $copied file(s); $errors error(s)."
if ($errors -gt 0) { exit 1 }
