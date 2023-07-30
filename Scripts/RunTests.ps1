$TestsAndCoveragePath = ".tests-and-coverage"

if (Test-Path $TestsAndCoveragePath)
{
    Remove-Item -Recurse -Force $TestsAndCoveragePath > $null
}

dotnet tool restore
dotnet test Todo.Analyzer.Tests -c Release --collect:"XPlat Code Coverage" --logger "html" --logger "xunit;LogFileName=todo.analyzer.{assembly}.test-results.xml" --results-directory "$TestsAndCoveragePath"
if(-not $?)
{
    Write-Host "Test failed" -ForegroundColor Red
    Exit 1
}

dotnet reportgenerator -reports:"$TestsAndCoveragePath/**/*.xml" -targetdir:"$TestsAndCoveragePath" -title:"Todo.Analyzer" -reporttypes:"Html;MarkdownSummary;XmlSummary" -assemblyfilters:"-Todo.Analyzer.CodeFixes" -classfilters:"-*Resources"
if(-not $?)
{
    Write-Host "Report failed" -ForegroundColor Red
    Exit 1
}

[xml]$Report = Get-Content -Raw "./$TestsAndCoveragePath/Summary.xml"
[double]$LineCoverage = [double]::Parse($Report.CoverageReport.Summary.LineCoverage)
[double]$Branchcoverage = [double]::Parse($Report.CoverageReport.Summary.Branchcoverage)
[double]$Methodcoverage = [double]::Parse($Report.CoverageReport.Summary.Methodcoverage)

if(($LineCoverage -lt 90.0) -or ($Branchcoverage -lt 90.0) -or ($Methodcoverage -lt 90.0))
{
    Write-Host "Poor coverage:" -ForegroundColor Red
    Write-Host " - Line Coverage: $LineCoverage" -ForegroundColor Red
    Write-Host " - Branch Coverage: $Branchcoverage" -ForegroundColor Red
    Write-Host " - Method Coverage: $Methodcoverage" -ForegroundColor Red
    Exit 1
}

Write-Host "Coverage:" -ForegroundColor Green
Write-Host " - Line Coverage: $LineCoverage %" -ForegroundColor Green
Write-Host " - Branch Coverage: $Branchcoverage %" -ForegroundColor Green
Write-Host " - Method Coverage: $Methodcoverage %" -ForegroundColor Green
Exit 0