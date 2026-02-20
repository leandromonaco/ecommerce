# Usage:
#   .\Start.ps1

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$UiPath = Join-Path $Root "src\ECommerce.UI"

# Install UI dependencies if node_modules is missing
if (-not (Test-Path (Join-Path $UiPath "node_modules"))) {
    Write-Host "node_modules not found — running npm install..." -ForegroundColor Yellow
    Push-Location $UiPath
    npm install
    Pop-Location
}

# Build the ordered list of projects to launch
$Projects = [System.Collections.Generic.List[hashtable]]::new()

$Projects.Add(@{
    Title   = "ECommerce.API"
    Path    = Join-Path $Root "src\ECommerce.API"
    Command = "dotnet run"
})

$Projects.Add(@{
    Title   = "ECommerce.Consumer"
    Path    = Join-Path $Root "src\ECommerce.Consumer"
    Command = "dotnet run"
})

$Projects.Add(@{
    Title   = "ECommerce.QueueTool"
    Path    = Join-Path $Root "src\ECommerce.QueueTool"
    Command = "dotnet run"
})

$Projects.Add(@{
    Title   = "ECommerce.UI"
    Path    = Join-Path $Root "src\ECommerce.UI"
    Command = "npm run dev"
})

# Launch — prefer Windows Terminal tabs, fall back to separate PowerShell windows
if (Get-Command wt -ErrorAction SilentlyContinue) {
    Write-Host "Launching in Windows Terminal tabs..." -ForegroundColor Cyan

    # Build a single wt argument string.
    # wt uses " ; " as a subcommand separator (parsed by wt, not by PowerShell).
    $wtParts = foreach ($p in $Projects) {
        "new-tab --title `"$($p.Title)`" -d `"$($p.Path)`" powershell -NoExit -Command `"$($p.Command)`""
    }

    Start-Process wt -ArgumentList ($wtParts -join " ; ")
}
else {
    Write-Host "Windows Terminal not found — launching separate PowerShell windows..." -ForegroundColor Cyan

    foreach ($p in $Projects) {
        Start-Process powershell -ArgumentList @(
            "-NoExit",
            "-Command",
            "Set-Location '$($p.Path)'; $($p.Command)"
        )
    }
}

Write-Host ""
Write-Host "Projects launched:" -ForegroundColor Green
foreach ($p in $Projects) {
    Write-Host "  $($p.Title)" -ForegroundColor White
}
Write-Host ""
Write-Host "Endpoints:" -ForegroundColor Green
Write-Host "  API     -> http://localhost:53001" -ForegroundColor White
Write-Host "  Swagger -> http://localhost:53001/swagger" -ForegroundColor White
Write-Host "  UI      -> http://localhost:5174" -ForegroundColor White
