using System;
using System.Text.Json;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizResourceTests {
    [TestMethod]
    public void Deserialize_ParsesAllFields() {
        string jsonString = """
        {
          "id": "res1",
          "name": "Resource 1",
          "type": "VM",
          "nativeType": "virtualMachine",
          "cloudPlatform": "Azure",
          "region": "eastus",
          "createdAt": "2024-01-01T00:00:00Z",
          "status": "Active",
          "publiclyAccessible": true,
          "hasPublicIpAddress": false,
          "isInternetFacing": true,
          "tags": { "env": "prod" },
          "cloudAccount": {
            "id": "acc1",
            "name": "Account",
            "cloudProvider": "Azure",
            "externalId": "sub1"
          },
          "securityGroups": ["sg-1", "sg-2"],
          "issues": {
            "criticalCount": 1,
            "highCount": 2,
            "mediumCount": 3,
            "lowCount": 4
          }
        }
        """;
        WizResource resource = JsonSerializer.Deserialize<WizResource>(jsonString, TestJson.Options)!;

        Assert.AreEqual("res1", resource.Id);
        Assert.AreEqual("Resource 1", resource.Name);
        Assert.AreEqual("VM", resource.Type);
        Assert.AreEqual("virtualMachine", resource.NativeType);
        Assert.AreEqual(WizCloudProvider.AZURE, resource.CloudPlatform);
        Assert.AreEqual("eastus", resource.Region);
        Assert.AreEqual(DateTime.Parse("2024-01-01T00:00:00Z").ToUniversalTime(), resource.CreatedAt);
        Assert.AreEqual("Active", resource.Status);
        Assert.IsTrue(resource.PubliclyAccessible);
        Assert.IsFalse(resource.HasPublicIpAddress);
        Assert.IsTrue(resource.IsInternetFacing);
        Assert.AreEqual("prod", resource.Tags["env"]);
        Assert.IsNotNull(resource.CloudAccount);
        Assert.AreEqual("acc1", resource.CloudAccount!.Id);
        Assert.AreEqual(2, resource.SecurityGroups.Count);
        Assert.IsNotNull(resource.Issues);
        Assert.AreEqual(1, resource.Issues!.CriticalCount);
    }
}
