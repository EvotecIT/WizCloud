using System.Text.Json.Nodes;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizCloudAccountTests {
    [TestMethod]
    public void FromJson_ParsesFields() {
        string jsonString = """
        {
          "id": "acc1",
          "name": "Account 1",
          "cloudProvider": "AWS",
          "externalId": "123456789012"
        }
        """;
        JsonNode json = JsonNode.Parse(jsonString)!;
        WizCloudAccount account = WizCloudAccount.FromJson(json);

        Assert.AreEqual("acc1", account.Id);
        Assert.AreEqual("Account 1", account.Name);
        Assert.AreEqual("AWS", account.CloudProvider);
        Assert.AreEqual("123456789012", account.ExternalId);
    }
}
