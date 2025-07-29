using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetUsersAsyncEnumerableTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizClient).GetMethod("GetUsersAsyncEnumerable");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(IAsyncEnumerable<WizUser>), method.ReturnType);
    }
}