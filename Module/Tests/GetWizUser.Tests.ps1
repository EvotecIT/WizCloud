Describe 'Get-WizUser cmdlet' {
    It 'streams results using await foreach' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletGetWizUser.cs') -Raw
        $source | Should -Match 'await foreach'
    }

    It 'handles HttpRequestException' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletGetWizUser.cs') -Raw
        $source | Should -Match 'HttpRequestException'
    }

    It 'defines Type and ProjectId parameters' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletGetWizUser.cs') -Raw
        $source | Should -Match 'ProjectId'
        $source | Should -Match 'WizUserType\[]'
    }
}