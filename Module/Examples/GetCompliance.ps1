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

# Get compliance posture
Write-Host "`nRetrieving compliance posture..." -ForegroundColor Yellow
$results = Get-WizCompliance -Verbose
Write-Host "`nFound $($results.Count) compliance results" -ForegroundColor Green
$results | Format-Table Framework, OverallScore
