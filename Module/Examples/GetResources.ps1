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

# Get resources
Write-Host "`nRetrieving resources..." -ForegroundColor Yellow
$resources = Get-WizResource -Verbose -MaxResults 50 -Type VM -CloudProvider AWS
Write-Host "`nFound $($resources.Count) resources" -ForegroundColor Green
$resources | Format-Table Id, Name, Type, CloudPlatform, Region
