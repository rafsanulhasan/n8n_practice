# Configure SonarCloud Quality Gate
# This script helps configure quality gate conditions for your project

$ErrorActionPreference = "Stop"

# Configuration
$SONAR_TOKEN = $env:SONAR_TOKEN
$ORGANIZATION = "rafsanulhasan"
$PROJECT_KEY = "rafsanulhasan_n8n_practice"
$SONAR_URL = "https://sonarcloud.io"

if (-not $SONAR_TOKEN) {
    Write-Error "SONAR_TOKEN environment variable is not set. Please set it first: `$env:SONAR_TOKEN='your_token'"
    exit 1
}

# Create authentication header
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("${SONAR_TOKEN}:"))
$headers = @{
    "Authorization" = "Basic $base64AuthInfo"
    "Content-Type" = "application/json"
}

Write-Host "Fetching current quality gate for project..." -ForegroundColor Cyan

# Get project's quality gate
try {
    $projectQGResponse = Invoke-RestMethod -Uri "$SONAR_URL/api/qualitygates/get_by_project?project=$PROJECT_KEY" -Headers $headers -Method Get
    $currentQualityGate = $projectQGResponse.qualityGate
    Write-Host "Current Quality Gate: $($currentQualityGate.name) (ID: $($currentQualityGate.id))" -ForegroundColor Green
} catch {
    Write-Host "Could not fetch quality gate: $_" -ForegroundColor Yellow
    Write-Host "The project might be using the default quality gate." -ForegroundColor Yellow
}

Write-Host "`nFetching all available quality gates..." -ForegroundColor Cyan

# List all quality gates
try {
    $qgListResponse = Invoke-RestMethod -Uri "$SONAR_URL/api/qualitygates/list" -Headers $headers -Method Get
    Write-Host "`nAvailable Quality Gates:" -ForegroundColor Green
    foreach ($qg in $qgListResponse.qualitygates) {
        $isDefault = if ($qg.isDefault) { " (DEFAULT)" } else { "" }
        Write-Host "  - $($qg.name) (ID: $($qg.id))$isDefault" -ForegroundColor White
    }
} catch {
    Write-Error "Failed to list quality gates: $_"
    exit 1
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "RECOMMENDED ACTIONS:" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Option 1: Use 'Sonar way' Quality Gate (Recommended)" -ForegroundColor Green
Write-Host "  This is the default quality gate with standard conditions."
Write-Host "  It typically requires 80% coverage on new code."
Write-Host ""
Write-Host "Option 2: Create a Custom Quality Gate" -ForegroundColor Green
Write-Host "  1. Go to: https://sonarcloud.io/organizations/$ORGANIZATION/quality_gates"
Write-Host "  2. Click 'Create'"
Write-Host "  3. Set conditions:"
Write-Host "     - Coverage on New Code: 50%"
Write-Host "     - Add other conditions as needed"
Write-Host "  4. Select your project and assign this quality gate"
Write-Host ""
Write-Host "Option 3: Modify Project Settings" -ForegroundColor Green
Write-Host "  Go to: https://sonarcloud.io/project/quality_gate?id=$PROJECT_KEY"
Write-Host "  And adjust the quality gate assignment"
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if user wants to set a specific quality gate
Write-Host "To associate your project with a specific quality gate, run:" -ForegroundColor Yellow
Write-Host "  Invoke-RestMethod -Uri '$SONAR_URL/api/qualitygates/select?projectKey=$PROJECT_KEY&gateName=<QualityGateName>' -Headers @{'Authorization'='Basic $base64AuthInfo'} -Method Post" -ForegroundColor White
Write-Host ""
