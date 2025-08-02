using System.Collections.Generic;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetNetworkExposuresAsyncTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizClient).GetMethod("GetNetworkExposuresAsync");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(Task<List<WizNetworkExposure>>), method!.ReturnType);
    }
}
