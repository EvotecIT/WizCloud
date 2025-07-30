using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizAuthenticationTests {
    [TestMethod]
    public async Task AcquireTokenAsync_WithNullClientId_ThrowsArgumentException() {
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await WizAuthentication.AcquireTokenAsync(null!, "secret", WizRegion.EU17));
    }

    [TestMethod]
    public async Task AcquireTokenAsync_WithNullClientSecret_ThrowsArgumentException() {
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await WizAuthentication.AcquireTokenAsync("id", null!, WizRegion.EU17));
    }

    [TestMethod]
    public void AcquireTokenAsync_ContainsDetailedErrorMessage() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud", "Authentication", "WizAuthentication.cs");
        var source = File.ReadAllText(filePath);
        StringAssert.Contains(source, "Authentication failed with status code");
    }
}