# Import the WizCloud module
Import-Module $PSScriptRoot\..\WizCloud.psd1 -Force

# Example 1: Connect to Wiz and get all projects
# NOTE: The region specified in Connect-Wiz will be used for all subsequent cmdlets
$connectWizSplat = @{
    ClientId       = $Env:WizClientID
    ClientSecret   = $Env:WizClientSecret
    TestConnection = $true
    Region         = 'eu17'
}

Connect-Wiz @connectWizSplat

# Get all projects
$Projects = Get-WizProject
Write-Host "Found $($Projects.Count) projects"

# Display project information
$Projects | Select-Object Name, Slug, IsFolder | Format-Table