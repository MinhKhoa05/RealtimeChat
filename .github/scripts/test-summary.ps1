$testSuites = @(
    @{
        Name = 'Unit'
        ResultsDirectory = 'artifacts/unit-test-results'
    },
    @{
        Name = 'Integration'
        ResultsDirectory = 'artifacts/integration-test-results'
    }
)

if (-not $env:GITHUB_STEP_SUMMARY) {
    Write-Host 'GITHUB_STEP_SUMMARY is not set. Skipping summary output.'
    return
}

function Get-TestSuiteResult {
    param(
        [string]$Name,
        [string]$ResultsDirectory
    )

    $trxFile = Get-ChildItem -Path $ResultsDirectory -Recurse -Filter '*.trx' -ErrorAction SilentlyContinue |
        Select-Object -First 1

    if (-not $trxFile) {
        return $null
    }

    [xml]$trx = Get-Content -LiteralPath $trxFile.FullName -Raw
    $counters = $trx.TestRun.ResultSummary.Counters
    $failed = [int]$counters.failed

    [pscustomobject]@{
        Suite = $Name
        Status = if ($failed -eq 0) { 'Passed' } else { 'Failed' }
        Passed = [int]$counters.passed
        Failed = $failed
        Skipped = [int]$counters.skipped
        Total = [int]$counters.total
    }
}

function Add-TestSummaryRow {
    param(
        [string]$Suite,
        [string]$Status,
        [int]$Passed,
        [int]$Failed,
        [int]$Skipped,
        [int]$Total,
        [switch]$Bold
    )

    if ($Bold) {
        "| **$Suite** | **$Status** | **$Passed** | **$Failed** | **$Skipped** | **$Total** |" >> $env:GITHUB_STEP_SUMMARY
        return
    }

    "| $Suite | $Status | $Passed | $Failed | $Skipped | $Total |" >> $env:GITHUB_STEP_SUMMARY
}

$results = foreach ($suite in $testSuites) {
    Get-TestSuiteResult -Name $suite.Name -ResultsDirectory $suite.ResultsDirectory
}

if (-not $results) {
    Write-Host 'No TRX files found. Skipping summary output.'
    return
}

$totalPassed = ($results | Measure-Object -Property Passed -Sum).Sum
$totalFailed = ($results | Measure-Object -Property Failed -Sum).Sum
$totalSkipped = ($results | Measure-Object -Property Skipped -Sum).Sum
$totalTests = ($results | Measure-Object -Property Total -Sum).Sum
$overallStatus = if ($totalFailed -eq 0) { 'Passed' } else { 'Failed' }

'## Test Summary' >> $env:GITHUB_STEP_SUMMARY
'' >> $env:GITHUB_STEP_SUMMARY
'| Suite | Status | Passed | Failed | Skipped | Total |' >> $env:GITHUB_STEP_SUMMARY
'| ----- | ------ | -----: | -----: | ------: | ----: |' >> $env:GITHUB_STEP_SUMMARY

foreach ($result in $results) {
    Add-TestSummaryRow `
        -Suite $result.Suite `
        -Status $result.Status `
        -Passed $result.Passed `
        -Failed $result.Failed `
        -Skipped $result.Skipped `
        -Total $result.Total
}

Add-TestSummaryRow `
    -Suite 'Total' `
    -Status $overallStatus `
    -Passed $totalPassed `
    -Failed $totalFailed `
    -Skipped $totalSkipped `
    -Total $totalTests `
    -Bold
