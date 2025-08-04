using System;
using System.IO;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizClientFactoryTests {
    [TestMethod]
    public void CreateAsync_ReturnsTaskOfWizClient() {
        var method = typeof(WizClient).GetMethod(
            "CreateAsync",
            new[] { typeof(string), typeof(string), typeof(WizRegion) });
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(Task<WizClient>), method!.ReturnType);
    }

    [TestMethod]
    public void CreateAsync_UsesAcquireTokenAsync() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud", "WizClient.cs");
        var source = File.ReadAllText(filePath);
        var methodIndex = source.IndexOf("CreateAsync(string clientId, string clientSecret", StringComparison.Ordinal);
        Assert.IsTrue(methodIndex >= 0, "CreateAsync method not found");
        var callIndex = source.IndexOf("AcquireTokenAsync", methodIndex, StringComparison.Ordinal);
        Assert.IsTrue(callIndex >= 0, "AcquireTokenAsync not used in CreateAsync");
    }
}
