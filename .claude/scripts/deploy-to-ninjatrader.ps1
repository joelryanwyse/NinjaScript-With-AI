# PostToolUse hook: copy a NinjaScript .cs file from the working directory
# to NinjaTrader 8's Custom\Indicators folder so changes are auto-deployed.
#
# Reads the tool payload JSON from stdin. Silent on success.
# On any failure, appends a line to %USERPROFILE%\.claude\nt-deploy-hook-errors.log
# and exits 1 (PostToolUse can't undo the edit, but the log surfaces the issue).
#
# Filtering: only files ending in .cs that live under $srcDir are deployed.
# Anything else exits 0 (no-op) so the hook never blocks unrelated edits.

$ErrorActionPreference = 'Stop'

try {
    $payload = [Console]::In.ReadToEnd()
    if ([string]::IsNullOrWhiteSpace($payload)) { exit 0 }

    # Some upstream paths feed this script a payload where backslashes
    # in file_path are not escaped (e.g. `C:\Users\...` instead of `C:\\Users\\...`).
    # That makes ConvertFrom-Json choke on "Unrecognized escape sequence" because
    # JSON requires `\\`. Pull file_path via regex first; fall back to ConvertFrom-Json.
    $src = $null
    $m = [regex]::Match($payload, '"file_path"\s*:\s*"([^"]+)"')
    if ($m.Success) {
        $src = $m.Groups[1].Value
    } else {
        $json = $payload | ConvertFrom-Json
        $src = $json.tool_input.file_path
    }
    if (-not $src) { exit 0 }

    # CLAUDE_PROJECT_DIR is set by Claude Code to the project root.
    $srcDir = $env:CLAUDE_PROJECT_DIR
    if (-not $srcDir) { exit 0 }

    # NinjaTrader 8 stores custom indicators under the user's Documents folder.
    $dstDir = Join-Path ([Environment]::GetFolderPath('MyDocuments')) 'NinjaTrader 8\bin\Custom\Indicators'

    $srcLower = $src.ToLower()
    if (-not $srcLower.EndsWith('.cs')) { exit 0 }
    if (-not $srcLower.StartsWith($srcDir.ToLower())) { exit 0 }

    if (-not (Test-Path -LiteralPath $src)) { exit 0 }
    if (-not (Test-Path -LiteralPath $dstDir)) { exit 0 }

    $file = Split-Path -Leaf $src
    $dst = Join-Path $dstDir $file

    Copy-Item -LiteralPath $src -Destination $dst -Force
    exit 0
}
catch {
    $logDir = Join-Path $env:USERPROFILE '.claude'
    if (-not (Test-Path -LiteralPath $logDir)) {
        New-Item -ItemType Directory -Force -Path $logDir | Out-Null
    }
    $logFile = Join-Path $logDir 'nt-deploy-hook-errors.log'
    $ts = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
    Add-Content -LiteralPath $logFile -Value "$ts ERROR: $($_.Exception.Message)"
    exit 1
}
