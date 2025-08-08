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
        $cmdlet.PageSize = 2
        $cmdlet.MaxResults = 1
        $cmdlet.Type = [WizCloud.WizUserType]::GROUP
        $cmdlet.ProjectId = 'proj1'

        $client = [WizCloud.WizClient]::new('token')
        $list = [System.Collections.Generic.List[WizCloud.WizUser]]::new()
        $list.Add([WizCloud.WizUser]::new())
        $list.Add([WizCloud.WizUser]::new())
        $captured = $null
        Mock -MemberName GetUsersAsyncEnumerable -Instance $client -MockWith {
            param($pageSize,$types,$projectId,$cancel)
            $script:captured = [pscustomobject]@{PageSize=$pageSize; Types=$types; ProjectId=$projectId}
            [TestAsyncEnumerable[WizCloud.WizUser]]::new($list)
        }
        $field = $cmdlet.GetType().GetField('_wizClient','NonPublic,Instance')
        $field.SetValue($cmdlet,$client)
        $script:output = @()
        Mock -MemberName WriteObject -Instance $cmdlet -MockWith { param($obj) $script:output += $obj }
        $method = $cmdlet.GetType().GetMethod('ProcessRecordAsync','NonPublic,Instance')
        $task = $method.Invoke($cmdlet,@())
        $task.GetAwaiter().GetResult()
        $script:output | Should -HaveCount 1
        $captured.PageSize | Should -Be 2
        $captured.Types | Should -Be (@([WizCloud.WizUserType]::GROUP))
        $captured.ProjectId | Should -Be 'proj1'
    }

    It 'restricts output to requested types' {
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizUser]::new()
        $cmdlet.Type = @([WizCloud.WizUserType]::USER_ACCOUNT, [WizCloud.WizUserType]::SERVICE_ACCOUNT)

        $client = [WizCloud.WizClient]::new('token')
        $list = [System.Collections.Generic.List[WizCloud.WizUser]]::new()
        $u1 = [WizCloud.WizUser]::new(); $u1.Type = [WizCloud.WizUserType]::USER_ACCOUNT; $list.Add($u1)
        $u2 = [WizCloud.WizUser]::new(); $u2.Type = [WizCloud.WizUserType]::SERVICE_ACCOUNT; $list.Add($u2)

        Mock -MemberName GetUsersAsyncEnumerable -Instance $client -MockWith {
            param($pageSize,$types,$projectId,$cancel)
            [TestAsyncEnumerable[WizCloud.WizUser]]::new($list)
        }
        $field = $cmdlet.GetType().GetField('_wizClient','NonPublic,Instance')
        $field.SetValue($cmdlet,$client)
        $script:output = @()
        Mock -MemberName WriteObject -Instance $cmdlet -MockWith { param($obj) $script:output += $obj }
        $method = $cmdlet.GetType().GetMethod('ProcessRecordAsync','NonPublic,Instance')
        $task = $method.Invoke($cmdlet,@())
        $task.GetAwaiter().GetResult()
        $script:output | Should -HaveCount 2
        ($script:output | ForEach-Object { $_.Type } | Sort-Object -Unique) | Should -Be (@([WizCloud.WizUserType]::USER_ACCOUNT, [WizCloud.WizUserType]::SERVICE_ACCOUNT))
    }

    It 'writes an error when WizClient throws HttpRequestException' {
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizUser]::new()
        $client = [WizCloud.WizClient]::new('token')
        Mock -MemberName GetUsersAsyncEnumerable -Instance $client -MockWith {
            param($pageSize,$types,$projectId,$cancel)
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

    It 'reports progress with total count when IncludeTotal is set' {
        $cmdlet = [WizCloud.PowerShell.CmdletGetWizUser]::new()
        $cmdlet.IncludeTotal = $true

        $client = [WizCloud.WizClient]::new('token')
        $list = [System.Collections.Generic.List[WizCloud.WizUser]]::new()
        $list.Add([WizCloud.WizUser]::new())
        $list.Add([WizCloud.WizUser]::new())

        Mock -MemberName GetUsersCountAsync -Instance $client -MockWith { 2 }
        Mock -MemberName GetUsersAsyncEnumerable -Instance $client -MockWith {
            param($pageSize,$types,$projectId,$cancel)
            [TestAsyncEnumerable[WizCloud.WizUser]]::new($list)
        }

        $field = $cmdlet.GetType().GetField('_wizClient','NonPublic,Instance')
        $field.SetValue($cmdlet,$client)

        $script:progress = @()
        Mock -MemberName WriteProgress -Instance $cmdlet -MockWith { param($pr) $script:progress += $pr }

        $method = $cmdlet.GetType().GetMethod('ProcessRecordAsync','NonPublic,Instance')
        $task = $method.Invoke($cmdlet,@())
        $task.GetAwaiter().GetResult()

        $script:progress | Should -Not -BeNullOrEmpty
        $script:progress[1].PercentComplete | Should -Be 50
        $script:progress[2].PercentComplete | Should -Be 100
        if ($script:progress[0].psobject.Properties['Total']) {
            $script:progress[0].Total | Should -Be 2
        }
        $script:progress[1].StatusDescription | Should -Be 'Retrieved 1 of 2 users...'
        $script:progress[2].StatusDescription | Should -Be 'Retrieved 2 of 2 users...'
    }
}
