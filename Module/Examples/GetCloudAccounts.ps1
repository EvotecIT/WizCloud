# Import the WizCloud module
Import-Module $PSScriptRoot\..\WizCloud.psd1 -Force

# Connect to Wiz
# NOTE: The region specified in Connect-Wiz will be used for all subsequent cmdlets
$connectWizSplat = @{
    ClientId       = $Env:WizClientID
    ClientSecret   = $Env:WizClientSecret
    TestConnection = $true
    Region         = 'eu17'
}
Connect-Wiz @connectWizSplat

# Get cloud accounts
Write-Host "`nRetrieving cloud accounts..." -ForegroundColor Yellow
$cloudAccounts = Get-WizCloudAccount -Verbose -MaxResults 50
Write-Host "`nFound $($cloudAccounts.Count) cloud accounts" -ForegroundColor Green
$cloudAccounts | Format-Table