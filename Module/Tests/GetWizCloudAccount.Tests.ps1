Describe 'Get-WizCloudAccount cmdlet' {
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
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizCloudAccount]::new()
        $cmdletType = $cmdlet.GetType()
        $binding = [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance
        $cmdlet.PageSize = 4
        $cmdlet.MaxResults = 1
        $client = [WizCloud.WizClient]::new('token')
        $list = [System.Collections.Generic.List[WizCloud.WizCloudAccount]]::new()
        $list.Add([WizCloud.WizCloudAccount]::new())
        $list.Add([WizCloud.WizCloudAccount]::new())
        $client = New-MockObject -InputObject $client -Methods @{
            GetCloudAccountsAsyncEnumerable = {
                param($pageSize,$cancel)
                [TestAsyncEnumerable[WizCloud.WizCloudAccount]]::new($list)
            }
        }
        $field = $cmdletType.GetField('_wizClient', $binding)
        $field.SetValue($cmdlet,$client)
        $method = $cmdletType.GetMethod('ProcessRecordAsync', $binding)
        $task = $method.Invoke($cmdlet,@())
        $task.GetAwaiter().GetResult()
    }

    It 'writes an error when WizClient throws HttpRequestException' {
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizCloudAccount]::new()
        $cmdletType = $cmdlet.GetType()
        $binding = [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance
        $client = [WizCloud.WizClient]::new('token')
        $client = New-MockObject -InputObject $client -Methods @{
            GetCloudAccountsAsyncEnumerable = {
                throw [System.Net.Http.HttpRequestException]::new('fail')
            }
        }
        $field = $cmdletType.GetField('_wizClient', $binding)
        $field.SetValue($cmdlet,$client)
        $method = $cmdletType.GetMethod('ProcessRecordAsync', $binding)
        $task = $method.Invoke($cmdlet,@())
        { $task.GetAwaiter().GetResult() } | Should -Not -Throw
    }
}
