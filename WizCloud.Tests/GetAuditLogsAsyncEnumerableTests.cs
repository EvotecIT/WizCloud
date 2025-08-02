using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetAuditLogsAsyncEnumerableTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizClient).GetMethod("GetAuditLogsAsyncEnumerable");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(IAsyncEnumerable<WizAuditLogEntry>), method!.ReturnType);
    }

    [TestMethod]
    public void MethodContainsErrorHandling() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var directory = Path.Combine(repoRoot, "WizCloud");
        var source = string.Concat(Directory.GetFiles(directory, "WizClient*.cs").Select(File.ReadAllText));
        var index = source.IndexOf("GetAuditLogsAsyncEnumerable", StringComparison.Ordinal);
        Assert.IsTrue(index >= 0, "GetAuditLogsAsyncEnumerable method not found");
        var snippet = source.Substring(index, Math.Min(1200, source.Length - index));
        StringAssert.Contains(snippet, "catch (HttpRequestException)");
        StringAssert.Contains(snippet, "yield break");
    }
}
