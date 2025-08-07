using System.Reflection;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WizCloud.Tests;

[TestClass]
public class BuildUserFiltersTests {
    private static MethodInfo GetMethod() =>
        typeof(WizCloud.WizClient).GetMethod("BuildUserFilters", BindingFlags.NonPublic | BindingFlags.Static)!;

    [TestMethod]
    public void BuildUserFilters_ReturnsNull_WhenNoFilters() {
        var method = GetMethod();
        var result = method.Invoke(null, new object?[] { null, null });
        Assert.IsNull(result);
    }

    [TestMethod]
    public void BuildUserFilters_UsesEquals_WhenSingleTypeProvided() {
        var method = GetMethod();
        var types = new[] { WizCloud.WizUserType.USER_ACCOUNT };
        var result = method.Invoke(null, new object?[] { types, null });
        var json = JsonSerializer.Serialize(result);
        StringAssert.Contains(json, "\"equals\":\"USER_ACCOUNT\"");
        Assert.IsFalse(json.Contains("equalsAnyOf"), json);
    }

    [TestMethod]
    public void BuildUserFilters_UsesEqualsAnyOf_WhenMultipleTypesProvided() {
        var method = GetMethod();
        var types = new[] { WizCloud.WizUserType.USER_ACCOUNT, WizCloud.WizUserType.SERVICE_ACCOUNT };
        var result = method.Invoke(null, new object?[] { types, null });
        var json = JsonSerializer.Serialize(result);
        StringAssert.Contains(json, "USER_ACCOUNT");
        StringAssert.Contains(json, "SERVICE_ACCOUNT");
        StringAssert.Contains(json, "equalsAnyOf");
    }

    [TestMethod]
    public void BuildUserFilters_IncludesProperty_WhenProjectIdProvided() {
        var method = GetMethod();
        var result = method.Invoke(null, new object?[] { null, "proj1" });
        var json = JsonSerializer.Serialize(result);
        StringAssert.Contains(json, "projectId");
        Assert.IsFalse(json.Contains("\"type\""), json);
    }
}
