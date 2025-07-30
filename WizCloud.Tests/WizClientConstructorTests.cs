using WizCloud;

namespace WizCloud.Tests;
[TestClass]
public sealed class WizClientConstructorTests {

    [TestMethod]
    public void HttpClient_IsSharedAcrossInstances() {
        using var first = new WizClient("token");
        using var second = new WizClient("token");

        var firstClientField = typeof(WizClient).GetField("_httpClient", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(firstClientField);
        var sharedInstance = firstClientField!.GetValue(null);
        Assert.IsNotNull(sharedInstance);
        var secondShared = firstClientField.GetValue(null);
        Assert.AreSame(sharedInstance, secondShared);
    }

    [TestMethod]
    public void Constructor_SetsRegionAndEndpoint() {
        using var client = new WizClient("token", WizRegion.US1);
        var endpointField = typeof(WizClient).GetField("_apiEndpoint", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(endpointField);
        string value = (string)endpointField!.GetValue(client)!;
        StringAssert.Contains(value, "us1");
    }
}