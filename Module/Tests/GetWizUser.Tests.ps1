Describe 'Get-WizUser cmdlet' {
    It 'has Stream parameter' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletGetWizUser.cs') -Raw
        $source | Should -Match 'SwitchParameter Stream'
    }
}