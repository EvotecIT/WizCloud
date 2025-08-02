using System.Text.Json.Nodes;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizIssueResourceTests {
    [TestMethod]
    public void FromJson_ParsesFields() {
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
        JsonNode json = JsonNode.Parse(jsonString)!;
        WizIssueResource resource = WizIssueResource.FromJson(json);

        Assert.AreEqual("res1", resource.Id);
        Assert.AreEqual("Resource", resource.Name);
        Assert.AreEqual("VM", resource.Type);
        Assert.AreEqual(WizCloudProvider.AWS, resource.CloudPlatform);
        Assert.AreEqual("us-east-1", resource.Region);
        Assert.AreEqual("sub1", resource.SubscriptionId);
    }
}
