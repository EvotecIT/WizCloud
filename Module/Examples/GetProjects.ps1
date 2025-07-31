# Import the WizCloud module
Import-Module .\WizCloud.psd1 -Force

# Example 1: Connect to Wiz and get all projects
Connect-Wiz -ClientId "clientId" -ClientSecret "clientSecret" -TestConnection

# Get all projects
$projects = Get-WizProject
Write-Host "Found $($projects.Count) projects"

# Display project information
$projects | Select-Object Name, Slug, IsFolder | Format-Table

# Example 2: Get projects with a specific page size
$projects = Get-WizProject -PageSize 50

# Example 3: Get projects from a specific region
$usProjects = Get-WizProject -Region [WizRegion]::US1 -Token "your-us-token"

# Example 4: Use Verbose to display progress
$null = Get-WizProject -Verbose
