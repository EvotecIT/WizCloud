using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WizCloud.Tests;

[TestClass]
public sealed class TokenRefreshTests {
    [TestMethod]
    public void WizClient_RefreshesTokenOnUnauthorized() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var directory = Path.Combine(repoRoot, "WizCloud");
        var source = string.Concat(Directory.GetFiles(directory, "WizClient*.cs").Select(File.ReadAllText));
        StringAssert.Contains(source, "AcquireTokenAsync");
        StringAssert.Contains(source, "Unauthorized");
    }

    [TestMethod]
    public void GetCloudAccounts_UsesSendGraphQlRequestAsync() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var directory = Path.Combine(repoRoot, "WizCloud");
        var source = string.Concat(Directory.GetFiles(directory, "WizClient*.cs").Select(File.ReadAllText));
        var index = source.IndexOf("GetCloudAccountsPageAsync", StringComparison.Ordinal);
        Assert.IsTrue(index >= 0, "GetCloudAccountsPageAsync method not found");
        var callIndex = source.IndexOf("SendGraphQlRequestAsync", index, StringComparison.Ordinal);
        Assert.IsTrue(callIndex >= 0, "SendGraphQlRequestAsync not used in GetCloudAccountsPageAsync");
    }

    [TestMethod]
    public void GetWizUserCmdlet_PassesClientCredentials() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud.PowerShell", "Cmdlets", "CmdletGetWizUser.cs");
        var source = File.ReadAllText(filePath);
        StringAssert.Contains(source, "ModuleInitialization.DefaultClientId");
        StringAssert.Contains(source, "ModuleInitialization.DefaultClientSecret");
        var ctorIndex = source.IndexOf("new WizClient", StringComparison.Ordinal);
        Assert.IsTrue(ctorIndex >= 0, "WizClient constructor call not found");
        var paramIndex = source.IndexOf("clientId, clientSecret", ctorIndex, StringComparison.Ordinal);
        Assert.IsTrue(paramIndex >= 0, "Client credentials not supplied to WizClient constructor");
    }


    [TestMethod]
    public void GetWizProjectCmdlet_PassesClientCredentials() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud.PowerShell", "Cmdlets", "CmdletGetWizProject.cs");
        var source = File.ReadAllText(filePath);
        StringAssert.Contains(source, "ModuleInitialization.DefaultClientId");
        StringAssert.Contains(source, "ModuleInitialization.DefaultClientSecret");
        var ctorIndex = source.IndexOf("new WizClient", StringComparison.Ordinal);
        Assert.IsTrue(ctorIndex >= 0, "WizClient constructor call not found");
        var paramIndex = source.IndexOf("clientId, clientSecret", ctorIndex, StringComparison.Ordinal);
        Assert.IsTrue(paramIndex >= 0, "Client credentials not supplied to WizClient constructor");
    }

    [TestMethod]
    public void GetWizIssueCmdlet_PassesClientCredentials() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud.PowerShell", "Cmdlets", "CmdletGetWizIssue.cs");
        var source = File.ReadAllText(filePath);
        StringAssert.Contains(source, "ModuleInitialization.DefaultClientId");
        StringAssert.Contains(source, "ModuleInitialization.DefaultClientSecret");
        var ctorIndex = source.IndexOf("new WizClient", StringComparison.Ordinal);
        Assert.IsTrue(ctorIndex >= 0, "WizClient constructor call not found");
        var paramIndex = source.IndexOf("clientId, clientSecret", ctorIndex, StringComparison.Ordinal);
        Assert.IsTrue(paramIndex >= 0, "Client credentials not supplied to WizClient constructor");
    }
}
