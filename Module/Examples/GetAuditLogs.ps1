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

# Get audit logs
Write-Host "`nRetrieving audit logs..." -ForegroundColor Yellow
$logs = Get-WizAuditLog -Verbose -MaxResults 50
Write-Host "`nFound $($logs.Count) audit logs" -ForegroundColor Green
$logs | Format-Table Timestamp, Action, Status

# Example: Use pipeline to get audit logs for specific users
Get-WizUser -MaxResults 1 | Get-WizAuditLog -MaxResults 10
