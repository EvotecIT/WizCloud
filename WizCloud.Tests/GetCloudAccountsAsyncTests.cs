using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetCloudAccountsAsyncTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizCloud.WizClient).GetMethod("GetCloudAccountsAsync");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(Task<List<WizCloud.WizCloudAccount>>), method!.ReturnType);
    }
}

