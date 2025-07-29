using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetProjectsAsyncTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizClient).GetMethod("GetProjectsAsync");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(Task<List<WizProject>>), method!.ReturnType);
    }
}
