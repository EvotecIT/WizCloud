# Import the WizCloud module
Import-Module $PSScriptRoot\..\WizCloud.psd1 -Force

# Example 1: Connect to Wiz and get all users
# NOTE: The region specified in Connect-Wiz will be used for all subsequent cmdlets
$connectWizSplat = @{
    ClientId       = $Env:WizClientID
    ClientSecret   = $Env:WizClientSecret
    TestConnection = $true
    Region         = 'eu17'
}
Connect-Wiz @connectWizSplat

# Get all users (uses the region from Connect-Wiz)
$users = Get-WizUser -Verbose -MaxResults 1200 -Type USER_ACCOUNT
Write-Host "Found $($users.Count) users"
$Users[1101] | Format-List

$Users | Format-Table Name, UserPrincipalName, UserType, HasMfa, ProjectNames, Email, IsActive, CreatedAt, LastSeenAt, LastLoginAt, LastLoginIpAddress, ProductNames, ProductIds

# Example 2: Get users for each project using the pipeline
Get-WizProject | Get-WizUser -MaxResults 10