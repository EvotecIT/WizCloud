Describe 'Get-WizCloudAccount cmdlet' {
    It 'streams results using await foreach' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletGetWizCloudAccount.cs') -Raw
        $source | Should -Match 'await foreach'
    }

    It 'handles HttpRequestException' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletGetWizCloudAccount.cs') -Raw
        $source | Should -Match 'HttpRequestException'
    }

    It 'defines PageSize and MaxResults parameters' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletGetWizCloudAccount.cs') -Raw
        $source | Should -Match 'PageSize'
        $source | Should -Match 'MaxResults'
    }
}
