Describe 'Get-WizProject cmdlet' {
    BeforeAll {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        Import-Module (Join-Path $repoRoot 'Module/WizCloud.psd1') -Force
        [WizCloud.PowerShell.ModuleInitialization]::DefaultToken = 'token'
        if (-not ([System.Management.Automation.PSTypeName]'TestAsyncEnumerable`1').Type) {
            Add-Type -Language CSharp @"
using System.Collections.Generic;
using System.Threading;
public class TestAsyncEnumerable<T> : IAsyncEnumerable<T> {
    private readonly IEnumerable<T> _items;
    public TestAsyncEnumerable(IEnumerable<T> items) => _items = items;
    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) {
        foreach (var item in _items) {
            yield return item;
        }
        await System.Threading.Tasks.Task.CompletedTask;
    }
}
"@
        }
    }

    It 'passes PageSize to WizClient and respects MaxResults' {
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizProject]::new()
        $cmdletType = $cmdlet.GetType()
        $binding = [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance
        $cmdlet.PageSize = 3
        $cmdlet.MaxResults = 1
        $client = [WizCloud.WizClient]::new('token')
        $list = [System.Collections.Generic.List[WizCloud.WizProject]]::new()
        $list.Add([WizCloud.WizProject]::new())
        $list.Add([WizCloud.WizProject]::new())
        $client = New-MockObject -InputObject $client -Methods @{
            GetProjectsAsyncEnumerable = {
                param($pageSize,$cancel)
                [TestAsyncEnumerable[WizCloud.WizProject]]::new($list)
            }
        }
        $field = $cmdletType.GetField('_wizClient', $binding)
        $field.SetValue($cmdlet,$client)
        $method = $cmdletType.GetMethod('ProcessRecordAsync', $binding)
        $task = $method.Invoke($cmdlet,@())
        $task.GetAwaiter().GetResult()
    }

    It 'writes an error when WizClient throws HttpRequestException' {
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizProject]::new()
        $cmdletType = $cmdlet.GetType()
        $binding = [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance
        $client = [WizCloud.WizClient]::new('token')
        $client = New-MockObject -InputObject $client -Methods @{
            GetProjectsAsyncEnumerable = {
                throw [System.Net.Http.HttpRequestException]::new('fail')
            }
        }
        $field = $cmdletType.GetField('_wizClient', $binding)
        $field.SetValue($cmdlet,$client)
        $method = $cmdletType.GetMethod('ProcessRecordAsync', $binding)
        $task = $method.Invoke($cmdlet,@())
        { $task.GetAwaiter().GetResult() } | Should -Not -Throw
    }

    It 'does not expose a Region parameter' {
        $repoRoot = Resolve-Path -Path "$PSScriptRoot/../.."
        $source = Get-Content -Path (Join-Path $repoRoot 'WizCloud.PowerShell/Cmdlets/CmdletGetWizProject.cs') -Raw
        $source | Should -Not -Match 'public\s+WizRegion'
    }
}

