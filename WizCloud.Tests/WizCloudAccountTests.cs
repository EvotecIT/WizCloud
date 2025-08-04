using System.Text.Json;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizCloudAccountTests {
    [TestMethod]
    public void Deserialize_ParsesFields() {
        string jsonString = """
        {
          "id": "acc1",
          "name": "Account 1",
          "cloudProvider": "AWS",
          "externalId": "123456789012"
        }
        """;
        WizCloudAccount account = JsonSerializer.Deserialize<WizCloudAccount>(jsonString, TestJson.Options)!;

        Assert.AreEqual("acc1", account.Id);
        Assert.AreEqual("Account 1", account.Name);
        Assert.AreEqual("AWS", account.CloudProvider);
        Assert.AreEqual("123456789012", account.ExternalId);
    }
}
