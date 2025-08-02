using System.Collections.Generic;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetCompliancePostureAsyncTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizClient).GetMethod("GetCompliancePostureAsync");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(Task<List<WizComplianceResult>>), method!.ReturnType);
    }
}
