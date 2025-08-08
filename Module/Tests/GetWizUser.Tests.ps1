Describe 'Get-WizUser cmdlet' {
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

    It 'passes parameters to WizClient and respects MaxResults' {
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizUser]::new()
        $cmdletType = $cmdlet.GetType()
        $binding = [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance
        $cmdlet.PageSize = 2
        $cmdlet.MaxResults = 1
        $cmdlet.Type = [WizCloud.WizUserType]::GROUP
        $cmdlet.ProjectId = 'proj1'

        $client = [WizCloud.WizClient]::new('token')
        $list = [System.Collections.Generic.List[WizCloud.WizUser]]::new()
        $list.Add([WizCloud.WizUser]::new())
        $list.Add([WizCloud.WizUser]::new())
        $client = New-MockObject -InputObject $client -Methods @{
            GetUsersWithProgressAsyncEnumerable = {
                param($pageSize,$types,$projectId,$maxResults,$includeTotal,$progress,$cancel)
                $take = if ($maxResults) { $maxResults } else { $list.Count }
                $subset = $list | Select-Object -First $take
                [TestAsyncEnumerable[WizCloud.WizUser]]::new($subset)
            }
        }
        $field = $cmdletType.GetField('_wizClient', $binding)
        $field.SetValue($cmdlet,$client)
        $method = $cmdletType.GetMethod('ProcessRecordAsync', $binding)
        $task = $method.Invoke($cmdlet,@())
        $task.GetAwaiter().GetResult()
    }

    It 'restricts output to requested types' {
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizUser]::new()
        $cmdletType = $cmdlet.GetType()
        $binding = [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance
        $cmdlet.Type = @([WizCloud.WizUserType]::USER_ACCOUNT, [WizCloud.WizUserType]::SERVICE_ACCOUNT)

        $client = [WizCloud.WizClient]::new('token')
        $list = [System.Collections.Generic.List[WizCloud.WizUser]]::new()
        $u1 = [WizCloud.WizUser]::new(); $u1.Type = [WizCloud.WizUserType]::USER_ACCOUNT; $list.Add($u1)
        $u2 = [WizCloud.WizUser]::new(); $u2.Type = [WizCloud.WizUserType]::SERVICE_ACCOUNT; $list.Add($u2)

        $client = New-MockObject -InputObject $client -Methods @{
            GetUsersWithProgressAsyncEnumerable = {
                param($pageSize,$types,$projectId,$maxResults,$includeTotal,$progress,$cancel)
                [TestAsyncEnumerable[WizCloud.WizUser]]::new($list)
            }
        }
        $field = $cmdletType.GetField('_wizClient', $binding)
        $field.SetValue($cmdlet,$client)
        $method = $cmdletType.GetMethod('ProcessRecordAsync', $binding)
        $task = $method.Invoke($cmdlet,@())
        $task.GetAwaiter().GetResult()
    }

    It 'writes an error when WizClient throws HttpRequestException' {
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizUser]::new()
        $cmdletType = $cmdlet.GetType()
        $binding = [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance
        $client = [WizCloud.WizClient]::new('token')
        $client = New-MockObject -InputObject $client -Methods @{
            GetUsersWithProgressAsyncEnumerable = {
                param($pageSize,$types,$projectId,$maxResults,$includeTotal,$progress,$cancel)
                throw [System.Net.Http.HttpRequestException]::new('fail')
            }
        }
        $field = $cmdletType.GetField('_wizClient', $binding)
        $field.SetValue($cmdlet,$client)
        $method = $cmdletType.GetMethod('ProcessRecordAsync', $binding)
        $task = $method.Invoke($cmdlet,@())
        { $task.GetAwaiter().GetResult() } | Should -Not -Throw
    }

    It 'reports progress with total count when IncludeTotal is set' {
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizUser]::new()
        $cmdletType = $cmdlet.GetType()
        $binding = [System.Reflection.BindingFlags]::NonPublic -bor [System.Reflection.BindingFlags]::Instance
        $cmdlet.IncludeTotal = $true

        $client = [WizCloud.WizClient]::new('token')
        $list = [System.Collections.Generic.List[WizCloud.WizUser]]::new()
        $list.Add([WizCloud.WizUser]::new())
        $list.Add([WizCloud.WizUser]::new())

        $client = New-MockObject -InputObject $client -Methods @{
            GetUsersWithProgressAsyncEnumerable = {
                param($pageSize,$types,$projectId,$maxResults,$includeTotal,$progress,$cancel)
                $progress.Report([WizCloud.WizProgress]::new(0,2))
                $progress.Report([WizCloud.WizProgress]::new(1,2))
                $progress.Report([WizCloud.WizProgress]::new(2,2))
                [TestAsyncEnumerable[WizCloud.WizUser]]::new($list)
            }
        }

        $field = $cmdletType.GetField('_wizClient', $binding)
        $field.SetValue($cmdlet,$client)
        $method = $cmdletType.GetMethod('ProcessRecordAsync', $binding)
        $task = $method.Invoke($cmdlet,@())
        { $task.GetAwaiter().GetResult() } | Should -Not -Throw
    }
}