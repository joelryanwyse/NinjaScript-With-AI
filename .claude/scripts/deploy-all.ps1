# Manual safety-net deploy. Copies every root *.cs in the repo to
# NinjaTrader 8's Custom\Indicators folder.
#
# Usage:
#   .\.claude\scripts\deploy-all.ps1            # deploy all
#   .\.claude\scripts\deploy-all.ps1 -Quiet     # silent unless something fails
#   .\.claude\scripts\deploy-all.ps1 -Filter aiECT*   # only matching files
#
# Idempotent. Files that already match (same size + last-write time) are skipped.

[CmdletBinding()]
param(
    [string]$Filter = '*.cs',
    [switch]$Quiet
)

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
    throw "NinjaTrader Indicators folder not found at $dstDir. Is NT 8 installed for this Windows user?"
}

$files = Get-ChildItem -LiteralPath $srcDir -Filter $Filter -File
$copied = 0
$skipped = 0
$errors = 0

foreach ($f in $files) {
    $dst = Join-Path $dstDir $f.Name
    try {
        if (Test-Path -LiteralPath $dst) {
            $dstItem = Get-Item -LiteralPath $dst
            # NT regenerates a #region at end of deployed file, so DST can be larger.
            # Treat "same write time AND src not newer" as already-deployed.
            if ($dstItem.LastWriteTimeUtc -ge $f.LastWriteTimeUtc -and $dstItem.Length -ge $f.Length) {
                $skipped++
                if (-not $Quiet) { Write-Output "skip   $($f.Name)" }
                continue
            }
        }
        Copy-Item -LiteralPath $f.FullName -Destination $dst -Force
        $copied++
        if (-not $Quiet) { Write-Output "deploy $($f.Name)" }
    } catch {
        $errors++
        Write-Warning "FAILED $($f.Name): $($_.Exception.Message)"
    }
}

if (-not $Quiet -or $errors -gt 0) {
    Write-Output "Done. Deployed $copied, skipped $skipped, errors $errors."
}
if ($errors -gt 0) { exit 1 }
