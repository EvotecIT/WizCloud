Describe 'Get-WizProject cmdlet' {
    It 'streams results using await foreach' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletGetWizProject.cs') -Raw
        $source | Should -Match 'await foreach'
    }

    It 'handles HttpRequestException' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletGetWizProject.cs') -Raw
        $source | Should -Match 'HttpRequestException'
    }

    It 'does not expose a Region parameter' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletGetWizProject.cs') -Raw
        $source | Should -Not -Match 'public\s+WizRegion'
    }
}

