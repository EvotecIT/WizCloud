# Import the WizCloud module
Import-Module $PSScriptRoot\..\WizCloud.psd1 -Force

# Connect to Wiz
$connectWizSplat = @{
    ClientId       = $Env:WizClientID
    ClientSecret   = $Env:WizClientSecret
    TestConnection = $true
    Region         = 'eu17'
}
Connect-Wiz @connectWizSplat

# Get issues
Write-Host "`nRetrieving issues..." -ForegroundColor Yellow
$issues = Get-WizIssue -Verbose -MaxResults 50
Write-Host "`nFound $($issues.Count) issues" -ForegroundColor Green
$issues | Format-Table Id, Name, Severity, Status
