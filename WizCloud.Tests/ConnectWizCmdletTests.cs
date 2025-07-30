using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace WizCloud.Tests;

[TestClass]
public sealed class ConnectWizCmdletTests {
    [TestMethod]
    public void ConnectCmdlet_DoesNotUseTokenEnvironmentVariable() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud.PowerShell", "Cmdlets", "CmdletConnectWiz.cs");
        var source = File.ReadAllText(filePath);
        Assert.IsFalse(source.Contains("WIZ_SERVICE_ACCOUNT_TOKEN"));
    }

    [TestMethod]
    public void DisconnectCmdlet_Exists() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud.PowerShell", "Cmdlets", "CmdletDisconnectWiz.cs");
        Assert.IsTrue(File.Exists(filePath));
        var source = File.ReadAllText(filePath);
        StringAssert.Contains(source, "Disconnect, \"Wiz\"");
    }
}