# Import the WizCloud module
Import-Module .\WizCloud.psd1 -Force

# Example 1: Connect to Wiz and get all users
# Provide client credentials to acquire a token and store it for the session
Connect-Wiz -ClientId "clientId" -ClientSecret "clientSecret" -TestConnection

# Get all users
$users = Get-WizUser
Write-Host "Found $($users.Count) users"

# Display user information
$users | Select-Object Name, Type, HasAdminPrivileges, HasHighPrivileges | Format-Table

# Example 2: Get users with specific page size
$users = Get-WizUser -PageSize 50

# Example 3: Get users from a specific region
$usUsers = Get-WizUser -Region [WizRegion]::US1 -Token "your-us-token"

# Example 4: Filter users with admin privileges
$adminUsers = Get-WizUser | Where-Object { $_.HasAdminPrivileges -eq $true }
Write-Host "Found $($adminUsers.Count) admin users"

# Example 5: Filter only service accounts using the WizUserType enum
$serviceAccounts = Get-WizUser | Where-Object { $_.Type -eq [WizUserType]::SERVICE_ACCOUNT }
Write-Host "Found $($serviceAccounts.Count) service accounts"

# Example 6: Export users to CSV
Get-WizUser | Select-Object Name, Type, HasAdminPrivileges, @{N='Projects';E={$_.Projects.Count}} | Export-Csv -Path "WizUsers.csv" -NoTypeInformation

# Example 7: Show users with critical issues
$criticalUsers = Get-WizUser | Where-Object { $_.IssueAnalytics.CriticalSeverityCount -gt 0 }
$criticalUsers | ForEach-Object {
    Write-Host "$($_.Name) has $($_.IssueAnalytics.CriticalSeverityCount) critical issues"
}

# Example 8: Use Verbose to display progress and ensure it completes
$null = Get-WizUser -Verbose

# Example 9: Connect using client credentials
$null = Connect-Wiz -ClientId $env:WIZ_CLIENT_ID -ClientSecret $env:WIZ_CLIENT_SECRET -TestConnection

# Example 9: Stream users as they are retrieved
Get-WizUser | Select-Object Name, Type | Format-Table

# Example 10: Get only service accounts from a specific project
$serviceAccounts = Get-WizUser -Type SERVICE_ACCOUNT -ProjectId "project1"
Write-Host "Found $($serviceAccounts.Count) service accounts in project1"

