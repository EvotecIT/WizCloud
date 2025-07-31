using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetUsersPageAsyncTests {
    [TestMethod]
    public void Method_HasFilterParameters() {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var method = typeof(WizClient).GetMethod("GetUsersPageAsync", flags);
        Assert.IsNotNull(method);
        var parameters = method!.GetParameters();
        Assert.IsTrue(parameters.Length >= 4);
        Assert.AreEqual("types", parameters[2].Name);
        Assert.AreEqual("projectId", parameters[3].Name);
    }
}
