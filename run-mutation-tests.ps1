# Run Stryker Mutation Testing for N8n Webhook Client
# This script runs mutation testing on both unit and integration tests

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Starting Mutation Testing with Stryker" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Run mutation testing for Unit Tests
Write-Host "Running mutation tests for Unit Tests (Connector)..." -ForegroundColor Yellow
$projectPath = Resolve-Path "src/Connector/Connector.csproj" | Select-Object -ExpandProperty Path
Set-Location tests/Connector.UnitTests
dotnet stryker --project $projectPath
$unitTestResult = $LASTEXITCODE

# Return to root directory
Set-Location ../..

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Mutation Testing Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "NOTE: Integration tests for Blazor WebAssembly apps are skipped for mutation testing" -ForegroundColor Yellow
Write-Host "      as they are not compatible with Stryker.NET" -ForegroundColor Yellow
Write-Host ""

# Check results
if ($unitTestResult -eq 0) {
    Write-Host "[SUCCESS] All mutation tests passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "View reports at:" -ForegroundColor Cyan
    Write-Host "  - Unit Tests: tests/Connector.UnitTests/StrykerOutput/" -ForegroundColor White
    exit 0
} else {
    Write-Host "[FAILED] Some mutation tests failed or mutation score is below threshold!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Check the reports for details:" -ForegroundColor Yellow
    Write-Host "  - Unit Tests: tests/Connector.UnitTests/StrykerOutput/" -ForegroundColor White
    exit 1
}
