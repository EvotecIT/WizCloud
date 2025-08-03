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
        $cmdlet.PageSize = 3
        $cmdlet.MaxResults = 1
        $client = [WizCloud.WizClient]::new('token')
        $list = [System.Collections.Generic.List[WizCloud.WizProject]]::new()
        $list.Add([WizCloud.WizProject]::new())
        $list.Add([WizCloud.WizProject]::new())
        $captured = 0
        Mock -MemberName GetProjectsAsyncEnumerable -Instance $client -MockWith {
            param($pageSize,$cancel)
            $script:captured = $pageSize
            [TestAsyncEnumerable[WizCloud.WizProject]]::new($list)
        }
        $field = $cmdlet.GetType().GetField('_wizClient','NonPublic,Instance')
        $field.SetValue($cmdlet,$client)
        $script:output = @()
        Mock -MemberName WriteObject -Instance $cmdlet -MockWith { param($obj) $script:output += $obj }
        $method = $cmdlet.GetType().GetMethod('ProcessRecordAsync','NonPublic,Instance')
        $task = $method.Invoke($cmdlet,@())
        $task.GetAwaiter().GetResult()
        $script:output | Should -HaveCount 1
        $captured | Should -Be 3
    }

    It 'writes an error when WizClient throws HttpRequestException' {
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizProject]::new()
        $client = [WizCloud.WizClient]::new('token')
        Mock -MemberName GetProjectsAsyncEnumerable -Instance $client -MockWith {
            throw [System.Net.Http.HttpRequestException]::new('fail')
        }
        $field = $cmdlet.GetType().GetField('_wizClient','NonPublic,Instance')
        $field.SetValue($cmdlet,$client)
        $script:err = $null
        Mock -MemberName WriteError -Instance $cmdlet -MockWith { param($e) $script:err = $e }
        $method = $cmdlet.GetType().GetMethod('ProcessRecordAsync','NonPublic,Instance')
        $task = $method.Invoke($cmdlet,@())
        $task.GetAwaiter().GetResult()
        $script:err.Exception | Should -BeOfType ([System.Net.Http.HttpRequestException])
    }
}
