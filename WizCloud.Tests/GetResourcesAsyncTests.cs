using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetResourcesAsyncTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizClient).GetMethod("GetResourcesAsync");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(Task<List<WizResource>>), method!.ReturnType);
    }
}
