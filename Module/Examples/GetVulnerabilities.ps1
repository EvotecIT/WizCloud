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

# Get vulnerabilities
Write-Host "`nRetrieving vulnerabilities..." -ForegroundColor Yellow
$vulnerabilities = Get-WizVulnerability -Verbose -MaxResults 50
Write-Host "`nFound $($vulnerabilities.Count) vulnerabilities" -ForegroundColor Green
$vulnerabilities | Select-Object Id, Cve, @{Name='Cvss';Expression={$_.Cvss.Score}}, ExploitAvailable | Format-Table

