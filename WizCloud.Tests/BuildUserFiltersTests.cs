using System.Reflection;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WizCloud.Tests;

[TestClass]
public class BuildUserFiltersTests {
    [TestMethod]
    public void BuildUserFilters_OmitsProperty_WhenProjectIdNull() {
        var method = typeof(WizCloud.WizClient).GetMethod("BuildUserFilters", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        var result = method!.Invoke(null, new object?[] { null, null });
        var json = JsonSerializer.Serialize(result);
        Assert.IsFalse(json.Contains("property"), json);
    }

    [TestMethod]
    public void BuildUserFilters_IncludesProperty_WhenProjectIdProvided() {
        var method = typeof(WizCloud.WizClient).GetMethod("BuildUserFilters", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.IsNotNull(method);
        var result = method!.Invoke(null, new object?[] { null, "proj1" });
        var json = JsonSerializer.Serialize(result);
        StringAssert.Contains(json, "projectId");
    }
}
