#!/usr/bin/env pwsh
# Check Mutation Testing Score
param([string]$StrykerOutputPath = "tests/Connector.UnitTests/StrykerOutput", [int]$Threshold = 80)
Write-Host "========================================"
Write-Host "Checking Mutation Testing Score"
Write-Host "========================================"
Write-Host ""
$latestReport = Get-ChildItem -Path $StrykerOutputPath -Directory -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $latestReport) { Write-Host "No StrykerOutput directory found"; exit 0 }
$reportPath = Join-Path $latestReport.FullName "reports/mutation-report.json"
if (-not (Test-Path $reportPath)) { Write-Host "Mutation report not found"; exit 0 }
$report = Get-Content $reportPath -Raw | ConvertFrom-Json
$totalMutants = 0; $killedMutants = 0; $survivedMutants = 0; $timeoutMutants = 0
foreach ($file in $report.files.PSObject.Properties) {
    foreach ($mutant in $file.Value.mutants) {
        if ($mutant.status -ne "Ignored") {
            $totalMutants++
            if ($mutant.status -eq "Killed") { $killedMutants++ }
            elseif ($mutant.status -eq "Timeout") { $timeoutMutants++; $killedMutants++ }
            elseif ($mutant.status -eq "Survived") { $survivedMutants++ }
        }
    }
}
if ($totalMutants -eq 0) { Write-Host "No testable mutants found"; exit 0 }
$mutationScore = [math]::Round(($killedMutants / $totalMutants) * 100, 2)
Write-Host "Mutation Testing Results:"
Write-Host "  Total Mutants:    $totalMutants"
Write-Host "  Killed:           $killedMutants"
Write-Host "  Survived:         $survivedMutants"
Write-Host "  Timeout:          $timeoutMutants"
Write-Host ""
Write-Host "Mutation Score:   $mutationScore%"
Write-Host "Threshold:        $Threshold%"
Write-Host ""
if ($mutationScore -lt $Threshold) {
    Write-Host "[FAILED] Mutation score is below threshold"
    exit 1
} else {
    Write-Host "[PASSED] Mutation score meets threshold"
    exit 0
}
