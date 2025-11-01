<#
.SYNOPSIS
    Runs tests for DKSKMaui with code coverage and generates HTML reports
.DESCRIPTION
    This script executes all tests, collects code coverage, and generates an HTML report.
    It can optionally open the report in your default browser.
.PARAMETER OpenReport
    If specified, opens the coverage report in the default browser after generation
.PARAMETER TestFilter
    Optional test filter expression (e.g., "Category=Unit")
.EXAMPLE
    .\run-tests.ps1
    .\run-tests.ps1 -OpenReport
    .\run-tests.ps1 -TestFilter "FullyQualifiedName~CompanyService"
#>

param(
    [switch]$OpenReport,
    [string]$TestFilter = ""
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "DKSKMaui Test Runner with Coverage" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Clean previous test results
if (Test-Path "./TestResults") {
    Write-Host "Cleaning previous test results..." -ForegroundColor Yellow
    Remove-Item -Path "./TestResults" -Recurse -Force
}

# Build the test project
Write-Host "Building test project..." -ForegroundColor Green
dotnet build --configuration Release --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Run tests with coverage
Write-Host ""
Write-Host "Running tests with coverage collection..." -ForegroundColor Green

$testCommand = "dotnet test --no-build --configuration Release --collect:`"XPlat Code Coverage`" --settings coverlet.runsettings --logger:`"console;verbosity=normal`""

if ($TestFilter -ne "") {
    $testCommand += " --filter `"$TestFilter`""
    Write-Host "Using filter: $TestFilter" -ForegroundColor Cyan
}

Invoke-Expression $testCommand

if ($LASTEXITCODE -ne 0) {
    Write-Host "Tests failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Find the coverage file
$coverageFile = Get-ChildItem -Path "./TestResults" -Filter "coverage.cobertura.xml" -Recurse | Select-Object -First 1

if ($null -eq $coverageFile) {
    Write-Host "Coverage file not found!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Coverage file found: $($coverageFile.FullName)" -ForegroundColor Green

# Check if ReportGenerator is installed
$reportGeneratorPath = Get-Command reportgenerator -ErrorAction SilentlyContinue

if ($null -eq $reportGeneratorPath) {
    Write-Host ""
    Write-Host "Installing ReportGenerator tool..." -ForegroundColor Yellow
    dotnet tool install -g dotnet-reportgenerator-globaltool
}

# Generate HTML report
Write-Host ""
Write-Host "Generating HTML coverage report..." -ForegroundColor Green

reportgenerator `
    -reports:"$($coverageFile.FullName)" `
    -targetdir:"./TestResults/CoverageReport" `
    -reporttypes:"Html;HtmlSummary;Cobertura;Badges" `
    -title:"DKSKMaui Test Coverage" `
    -assemblyfilters:"+DKSKMaui;-DKSKMaui.Tests" `
    -classfilters:"-*.Migrations.*;-*Program;-*MauiProgram" `
    -verbosity:"Warning"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Report generation failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Test execution completed successfully!" -ForegroundColor Green
Write-Host "Coverage report: ./TestResults/CoverageReport/index.html" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green

# Display coverage summary
$summaryFile = "./TestResults/CoverageReport/Summary.txt"
if (Test-Path $summaryFile) {
    Write-Host ""
    Write-Host "Coverage Summary:" -ForegroundColor Cyan
    Get-Content $summaryFile | Write-Host
}

# Open report if requested
if ($OpenReport) {
    Write-Host ""
    Write-Host "Opening coverage report in browser..." -ForegroundColor Cyan
    Start-Process "./TestResults/CoverageReport/index.html"
}
