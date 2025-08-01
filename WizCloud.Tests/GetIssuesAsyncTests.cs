using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetIssuesAsyncTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizClient).GetMethod("GetIssuesAsync");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(Task<List<WizIssue>>), method!.ReturnType);
    }
}
