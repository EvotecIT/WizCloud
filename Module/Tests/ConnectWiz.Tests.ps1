Describe 'Connect-Wiz cmdlet' {
    It 'handles HttpRequestException' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletConnectWiz.cs') -Raw
        $source | Should -Match 'HttpRequestException'
    }

    It 'defines exclusive parameter sets' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletConnectWiz.cs') -Raw
        $source | Should -Match 'ParameterSetName\s*=\s*TokenParameterSet'
        $source | Should -Match 'ParameterSetName\s*=\s*ClientCredentialParameterSet'
    }
}
