# Import the WizCloud module
Import-Module .\WizCloud.psd1 -Force

# Example 1: Connect to Wiz and get all users
# First, set your token as an environment variable or pass it directly
# $env:WIZ_SERVICE_ACCOUNT_TOKEN = "your-token-here"

# Connect to Wiz (uses environment variable if token not provided)
Connect-Wiz -TestConnection

# Get all users
$users = Get-WizUser
Write-Host "Found $($users.Count) users"

# Display user information
$users | Select-Object Name, Type, HasAdminPrivileges, HasHighPrivileges | Format-Table

# Example 2: Get users with specific page size
$users = Get-WizUser -PageSize 50

# Example 3: Get users from a specific region
$usUsers = Get-WizUser -Region "us1" -Token "your-us-token"

# Example 4: Filter users with admin privileges
$adminUsers = Get-WizUser | Where-Object { $_.HasAdminPrivileges -eq $true }
Write-Host "Found $($adminUsers.Count) admin users"

# Example 5: Export users to CSV
Get-WizUser | Select-Object Name, Type, HasAdminPrivileges, @{N='Projects';E={$_.Projects.Count}} | Export-Csv -Path "WizUsers.csv" -NoTypeInformation

# Example 6: Show users with critical issues
$criticalUsers = Get-WizUser | Where-Object { $_.IssueAnalytics.CriticalSeverityCount -gt 0 }
$criticalUsers | ForEach-Object {
    Write-Host "$($_.Name) has $($_.IssueAnalytics.CriticalSeverityCount) critical issues"
}

# Example 7: Use Verbose to display progress and ensure it completes
$null = Get-WizUser -Verbose

# Example 8: Connect using client credentials
$null = Connect-Wiz -ClientId $env:WIZ_CLIENT_ID -ClientSecret $env:WIZ_CLIENT_SECRET -TestConnection

# Example 9: Stream users as they are retrieved
Get-WizUser | Select-Object Name, Type | Format-Table

