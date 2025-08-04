Describe 'Cmdlet pipeline metadata' {
    BeforeAll {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        Import-Module (Join-Path $repoRoot 'Module/WizCloud.psd1') -Force
    }

    It 'CmdletGetWizUser ProjectId accepts pipeline by property name' {
        $attr = [WizCloud.PowerShell.CmdletGetWizUser].GetProperty('ProjectId').GetCustomAttributes([System.Management.Automation.ParameterAttribute], $false)[0]
        $attr.ValueFromPipelineByPropertyName | Should -BeTrue
    }

    It 'CmdletGetWizResource ProjectId accepts pipeline by property name' {
        $attr = [WizCloud.PowerShell.CmdletGetWizResource].GetProperty('ProjectId').GetCustomAttributes([System.Management.Automation.ParameterAttribute], $false)[0]
        $attr.ValueFromPipelineByPropertyName | Should -BeTrue
    }

    It 'CmdletGetWizIssue ProjectId accepts pipeline by property name' {
        $attr = [WizCloud.PowerShell.CmdletGetWizIssue].GetProperty('ProjectId').GetCustomAttributes([System.Management.Automation.ParameterAttribute], $false)[0]
        $attr.ValueFromPipelineByPropertyName | Should -BeTrue
    }

    It 'CmdletGetWizNetworkExposure ProjectId accepts pipeline by property name' {
        $attr = [WizCloud.PowerShell.CmdletGetWizNetworkExposure].GetProperty('ProjectId').GetCustomAttributes([System.Management.Automation.ParameterAttribute], $false)[0]
        $attr.ValueFromPipelineByPropertyName | Should -BeTrue
    }

    It 'CmdletGetWizConfigurationFinding ProjectId accepts pipeline by property name' {
        $attr = [WizCloud.PowerShell.CmdletGetWizConfigurationFinding].GetProperty('ProjectId').GetCustomAttributes([System.Management.Automation.ParameterAttribute], $false)[0]
        $attr.ValueFromPipelineByPropertyName | Should -BeTrue
    }

    It 'CmdletGetWizVulnerability ProjectId accepts pipeline by property name' {
        $attr = [WizCloud.PowerShell.CmdletGetWizVulnerability].GetProperty('ProjectId').GetCustomAttributes([System.Management.Automation.ParameterAttribute], $false)[0]
        $attr.ValueFromPipelineByPropertyName | Should -BeTrue
    }

    It 'CmdletGetWizAuditLog User accepts pipeline by value and property name' {
        $attr = [WizCloud.PowerShell.CmdletGetWizAuditLog].GetProperty('User').GetCustomAttributes([System.Management.Automation.ParameterAttribute], $false)[0]
        $attr.ValueFromPipeline | Should -BeTrue
        $attr.ValueFromPipelineByPropertyName | Should -BeTrue
    }
}
