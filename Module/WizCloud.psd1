@{
    AliasesToExport      = @()
    Author               = 'Przemyslaw Klys'
    CmdletsToExport      = @('Connect-Wiz', 'Disconnect-Wiz', 'Get-WizAuditLog', 'Get-WizCloudAccount', 'Get-WizCompliance', 'Get-WizConfigurationFinding', 'Get-WizIssue', 'Get-WizNetworkExposure', 'Get-WizProject', 'Get-WizResource', 'Get-WizUser', 'Get-WizVulnerability')
    CompanyName          = 'Evotec'
    CompatiblePSEditions = @('Desktop', 'Core')
    Copyright            = '(c) 2011 - 2025 Przemyslaw Klys @ Evotec. All rights reserved.'
    Description          = 'WizCloud is a PowerShell module that provides a set of cmdlets for managing and interacting with Wiz.io service'
    FunctionsToExport    = @()
    GUID                 = 'c6d394bc-7494-4619-b006-a073a6e76598'
    ModuleVersion        = '1.0.0'
    PowerShellVersion    = '5.1'
    PrivateData          = @{
        PSData = @{
            ProjectUri = 'https://github.com/EvotecIT/WizCloud'
            Tags       = @('Windows', 'MacOS', 'Linux')
        }
    }
    RootModule           = 'WizCloud.psm1'
}