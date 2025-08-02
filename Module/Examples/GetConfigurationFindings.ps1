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

# Get configuration findings
Write-Host "`nRetrieving configuration findings..." -ForegroundColor Yellow
$findings = Get-WizConfigurationFinding -Verbose -MaxResults 50
Write-Host "`nFound $($findings.Count) findings" -ForegroundColor Green
$findings | Format-Table Id, Title, Severity
