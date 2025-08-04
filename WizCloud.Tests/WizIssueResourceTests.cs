using System.Text.Json;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizIssueResourceTests {
    [TestMethod]
    public void Deserialize_ParsesFields() {
        string jsonString = """
        {
          "id": "res1",
          "name": "Resource",
          "type": "VM",
          "cloudPlatform": "AWS",
          "region": "us-east-1",
          "subscriptionId": "sub1"
        }
        """;
        WizIssueResource resource = JsonSerializer.Deserialize<WizIssueResource>(jsonString, TestJson.Options)!;

        Assert.AreEqual("res1", resource.Id);
        Assert.AreEqual("Resource", resource.Name);
        Assert.AreEqual("VM", resource.Type);
        Assert.AreEqual(WizCloudProvider.AWS, resource.CloudPlatform);
        Assert.AreEqual("us-east-1", resource.Region);
        Assert.AreEqual("sub1", resource.SubscriptionId);
    }
}
