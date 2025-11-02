# Run Stryker Mutation Testing for N8n Webhook Client
# This script runs mutation testing on both unit and integration tests

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Starting Mutation Testing with Stryker" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Run mutation testing for Unit Tests
Write-Host "Running mutation tests for Unit Tests..." -ForegroundColor Yellow
Set-Location n8n-blazor-frontend/N8nWebhookClient.UnitTests
dotnet stryker
$unitTestResult = $LASTEXITCODE

Write-Host ""
Write-Host "----------------------------------------" -ForegroundColor Cyan

# Run mutation testing for Integration Tests
Write-Host "Running mutation tests for Integration Tests..." -ForegroundColor Yellow
Set-Location ../N8nWebhookClient.IntegrationTests
dotnet stryker
$integrationTestResult = $LASTEXITCODE

# Return to root directory
Set-Location ../..

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Mutation Testing Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check results
if ($unitTestResult -eq 0 -and $integrationTestResult -eq 0) {
    Write-Host "✓ All mutation tests passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "View reports at:" -ForegroundColor Cyan
    Write-Host "  - Unit Tests: n8n-blazor-frontend/N8nWebhookClient.UnitTests/StrykerOutput/" -ForegroundColor White
    Write-Host "  - Integration Tests: n8n-blazor-frontend/N8nWebhookClient.IntegrationTests/StrykerOutput/" -ForegroundColor White
    exit 0
} else {
    Write-Host "✗ Some mutation tests failed or mutation score is below threshold!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Check the reports for details:" -ForegroundColor Yellow
    Write-Host "  - Unit Tests: n8n-blazor-frontend/N8nWebhookClient.UnitTests/StrykerOutput/" -ForegroundColor White
    Write-Host "  - Integration Tests: n8n-blazor-frontend/N8nWebhookClient.IntegrationTests/StrykerOutput/" -ForegroundColor White
    exit 1
}
