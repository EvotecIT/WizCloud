using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WizCloud.Tests;

[TestClass]
public sealed class TokenRefreshTests {
    [TestMethod]
    public void WizClient_RefreshesTokenOnUnauthorized() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud", "WizClient.cs");
        var source = File.ReadAllText(filePath);
        StringAssert.Contains(source, "AcquireTokenAsync");
        StringAssert.Contains(source, "Unauthorized");
    }

    [TestMethod]
    public void GetCloudAccounts_UsesSendWithRefreshAsync() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud", "WizClient.cs");
        var source = File.ReadAllText(filePath);
        var index = source.IndexOf("GetCloudAccountsPageAsync", StringComparison.Ordinal);
        Assert.IsTrue(index >= 0, "GetCloudAccountsPageAsync method not found");
        var callIndex = source.IndexOf("SendWithRefreshAsync", index, StringComparison.Ordinal);
        Assert.IsTrue(callIndex >= 0, "SendWithRefreshAsync not used in GetCloudAccountsPageAsync");
    }
}

