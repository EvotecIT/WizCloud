using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizAuthenticationTests
{
    [TestMethod]
    public async Task AcquireTokenAsync_WithNullClientId_ThrowsArgumentException()
    {
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await WizAuthentication.AcquireTokenAsync(null!, "secret", WizRegion.EU17));
    }

    [TestMethod]
    public async Task AcquireTokenAsync_WithNullClientSecret_ThrowsArgumentException()
    {
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await WizAuthentication.AcquireTokenAsync("id", null!, WizRegion.EU17));
    }
}
