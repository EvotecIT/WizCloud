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

# Get network exposure
Write-Host "`nRetrieving network exposure..." -ForegroundColor Yellow
$exposures = Get-WizNetworkExposure -Verbose -MaxResults 50
Write-Host "`nFound $($exposures.Count) exposures" -ForegroundColor Green
$exposures | Format-Table Id, ExposureType, PublicIpAddress

# Example: Use pipeline to get exposures by project
Get-WizProject | Get-WizNetworkExposure -MaxResults 10
