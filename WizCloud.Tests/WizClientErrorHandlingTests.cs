using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizClientErrorHandlingTests {
    [TestMethod]
    public void WizClient_ContainsDetailedErrorMessageInSingleLocation() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var directory = Path.Combine(repoRoot, "WizCloud");
        var mainFile = Path.Combine(directory, "WizClient.cs");
        var mainSource = File.ReadAllText(mainFile);
        StringAssert.Contains(mainSource, "Request failed with status code");

        var otherFiles = Directory.GetFiles(directory, "WizClient*.cs")
            .Where(f => !f.EndsWith("WizClient.cs", StringComparison.Ordinal));
        foreach (var file in otherFiles)
            Assert.IsFalse(File.ReadAllText(file).Contains("Request failed with status code"),
                $"Error message should be centralized, but found in {Path.GetFileName(file)}");
    }

    [TestMethod]
    public void WizClient_HasSendGraphQlRequestAsyncMethod() {
        var method = typeof(WizClient).GetMethod(
            "SendGraphQlRequestAsync",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(method);
    }
}
