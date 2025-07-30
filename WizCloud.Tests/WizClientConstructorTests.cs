using WizCloud;

namespace WizCloud.Tests;
[TestClass]
public sealed class WizClientConstructorTests {
    [TestMethod]
    public void Constructor_WithValidRegion_Succeeds() {
        using var client = new WizClient("token", WizRegion.US1);
        Assert.IsNotNull(client);
    }

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
}