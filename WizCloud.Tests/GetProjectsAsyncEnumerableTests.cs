using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetProjectsAsyncEnumerableTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizClient).GetMethod("GetProjectsAsyncEnumerable");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(IAsyncEnumerable<WizProject>), method!.ReturnType);
    }
}