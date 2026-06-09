$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$defaultLogPath = Join-Path $repoRoot "lab-5\agent_log.txt"
$logPath = if ([string]::IsNullOrWhiteSpace($env:CODEX_HOOK_LOG_PATH)) {
    $defaultLogPath
} else {
    $env:CODEX_HOOK_LOG_PATH
}

$logDir = Split-Path -Parent $logPath
if (-not (Test-Path -LiteralPath $logDir)) {
    New-Item -ItemType Directory -Path $logDir -Force | Out-Null
}

$stdin = [Console]::In.ReadToEnd()
if ([string]::IsNullOrWhiteSpace($stdin)) {
    return
}

try {
    $payload = $stdin | ConvertFrom-Json

    if (-not $payload.PSObject.Properties.Name.Contains("timestamp")) {
        $payload | Add-Member -NotePropertyName "timestamp" -NotePropertyValue (Get-Date -Format "yyyy-MM-ddTHH:mm:ss.fffK")
    }

    $line = $payload | ConvertTo-Json -Compress -Depth 100
} catch {
    $line = $stdin.Trim()
}

Add-Content -LiteralPath $logPath -Value $line
