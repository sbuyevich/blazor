param(
    [string]$Url = "http://localhost:5000"
)

$exePath = Join-Path $PSScriptRoot "dist\student-projects.exe"

if (-not (Test-Path -LiteralPath $exePath)) {
    Write-Error "Published app not found at $exePath. Run 'dotnet publish -c Release -o dist' first."
    exit 1
}

$workingDirectory = Split-Path -Path $exePath -Parent
$process = Start-Process -FilePath $exePath -WorkingDirectory $workingDirectory -PassThru

Start-Sleep -Seconds 3
Start-Process $Url

Write-Host "Started student-projects (PID $($process.Id)) and opened $Url"
