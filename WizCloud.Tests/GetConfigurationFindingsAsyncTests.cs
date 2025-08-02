using System.Collections.Generic;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetConfigurationFindingsAsyncTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizClient).GetMethod("GetConfigurationFindingsAsync");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(Task<List<WizConfigurationFinding>>), method!.ReturnType);
    }
}
