using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetResourcesAsyncEnumerableTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizClient).GetMethod("GetResourcesAsyncEnumerable");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(IAsyncEnumerable<WizResource>), method!.ReturnType);
    }

    [TestMethod]
    public void MethodContainsErrorHandling() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud", "WizClient.cs");
        var source = File.ReadAllText(filePath);
        var index = source.IndexOf("GetResourcesAsyncEnumerable", StringComparison.Ordinal);
        Assert.IsTrue(index >= 0, "GetResourcesAsyncEnumerable method not found");
        var snippet = source.Substring(index, Math.Min(1200, source.Length - index));
        StringAssert.Contains(snippet, "catch (HttpRequestException)");
        StringAssert.Contains(snippet, "yield break");
    }
}
