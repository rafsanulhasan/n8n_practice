#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Checks code coverage against a minimum threshold.

.DESCRIPTION
    This script parses OpenCover or Cobertura coverage reports and validates
    that the overall line coverage meets or exceeds the specified threshold.

.PARAMETER Threshold
    The minimum coverage percentage required (0-100). Default is 80.

.PARAMETER CoverageReportsPath
    Path to the coverage reports directory. Default is "./coverage".

.EXAMPLE
    .\check-code-coverage.ps1 -Threshold 80
    .\check-code-coverage.ps1 -Threshold 75 -CoverageReportsPath "./coverage"
#>

param(
    [Parameter(Mandatory = $false)]
    [ValidateRange(0, 100)]
    [int]$Threshold = 80,

    [Parameter(Mandatory = $false)]
    [string]$CoverageReportsPath = "./coverage"
)

$ErrorActionPreference = "Stop"

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Code Coverage Threshold Check" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Minimum Required: $Threshold%" -ForegroundColor Yellow
Write-Host ""

# Find all OpenCover XML files
$coverageFiles = Get-ChildItem -Path $CoverageReportsPath -Filter "coverage.opencover.xml" -Recurse -ErrorAction SilentlyContinue

if ($coverageFiles.Count -eq 0) {
    Write-Host "⚠️  No coverage reports found in '$CoverageReportsPath'" -ForegroundColor Yellow
    Write-Host "Searching for Cobertura reports instead..." -ForegroundColor Yellow
    
    $coverageFiles = Get-ChildItem -Path $CoverageReportsPath -Filter "coverage.cobertura.xml" -Recurse -ErrorAction SilentlyContinue
    
    if ($coverageFiles.Count -eq 0) {
        Write-Host "❌ No coverage reports found!" -ForegroundColor Red
        exit 1
    }
}

Write-Host "Found $($coverageFiles.Count) coverage report(s)" -ForegroundColor Green
Write-Host ""

$totalLinesCovered = 0
$totalLinesValid = 0
$totalBranchesCovered = 0
$totalBranchesValid = 0

foreach ($file in $coverageFiles) {
    Write-Host "Processing: $($file.FullName)" -ForegroundColor Gray
    
    try {
        [xml]$coverageXml = Get-Content $file.FullName
        
        # Check if it's OpenCover format
        if ($coverageXml.CoverageSession) {
            $summary = $coverageXml.CoverageSession.Summary
            
            if ($summary) {
                $linesCovered = [int]$summary.visitedSequencePoints
                $linesValid = [int]$summary.numSequencePoints
                $branchesCovered = [int]$summary.visitedBranchPoints
                $branchesValid = [int]$summary.numBranchPoints
                
                $totalLinesCovered += $linesCovered
                $totalLinesValid += $linesValid
                $totalBranchesCovered += $branchesCovered
                $totalBranchesValid += $branchesValid
                
                Write-Host "  Lines: $linesCovered / $linesValid" -ForegroundColor Gray
                Write-Host "  Branches: $branchesCovered / $branchesValid" -ForegroundColor Gray
            }
        }
        # Check if it's Cobertura format
        elseif ($coverageXml.coverage) {
            $lineRate = [double]$coverageXml.coverage.'line-rate'
            $branchRate = [double]$coverageXml.coverage.'branch-rate'
            
            Write-Host "  Line Rate: $($lineRate * 100)%" -ForegroundColor Gray
            Write-Host "  Branch Rate: $($branchRate * 100)%" -ForegroundColor Gray
            
            # For Cobertura, we use the rates directly
            $totalLinesCovered = $lineRate * 100
            $totalLinesValid = 100
        }
    }
    catch {
        Write-Host "  ⚠️  Failed to parse: $_" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Coverage Summary" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan

if ($totalLinesValid -eq 0) {
    Write-Host "❌ No valid coverage data found!" -ForegroundColor Red
    exit 1
}

$lineCoverage = [math]::Round(($totalLinesCovered / $totalLinesValid) * 100, 2)

Write-Host "Total Lines: $totalLinesValid" -ForegroundColor White
Write-Host "Covered Lines: $totalLinesCovered" -ForegroundColor White
Write-Host "Line Coverage: $lineCoverage%" -ForegroundColor $(if ($lineCoverage -ge $Threshold) { "Green" } else { "Red" })

if ($totalBranchesValid -gt 0) {
    $branchCoverage = [math]::Round(($totalBranchesCovered / $totalBranchesValid) * 100, 2)
    Write-Host "Branch Coverage: $branchCoverage%" -ForegroundColor White
}

Write-Host ""
Write-Host "Threshold: $Threshold%" -ForegroundColor Yellow
Write-Host ""

if ($lineCoverage -ge $Threshold) {
    Write-Host "✅ Coverage check PASSED! ($lineCoverage% >= $Threshold%)" -ForegroundColor Green
    Write-Host ""
    exit 0
}
else {
    $deficit = [math]::Round($Threshold - $lineCoverage, 2)
    Write-Host "❌ Coverage check FAILED!" -ForegroundColor Red
    Write-Host "   Current: $lineCoverage%" -ForegroundColor Red
    Write-Host "   Required: $Threshold%" -ForegroundColor Red
    Write-Host "   Deficit: $deficit%" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please add more tests to increase coverage." -ForegroundColor Yellow
    Write-Host ""
    exit 1
}
