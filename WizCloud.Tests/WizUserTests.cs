using System;
using System.Text.Json.Nodes;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizUserTests {
    [TestMethod]
    public void FromJson_ParsesAllFields() {
        string jsonString = """
{
  "id": "1",
  "name": "John Doe",
  "type": "USER_ACCOUNT",
  "nativeType": "AADUser",
  "deletedAt": "2024-05-01T00:00:00Z",
  "hasAccessToSensitiveData": true,
  "hasAdminPrivileges": false,
  "hasHighPrivileges": true,
  "hasSensitiveData": true,
  "graphEntity": {
    "id": "ge1",
    "type": "user",
    "properties": {
      "prop1": "value1"
    }
  },
  "projects": [
    {
      "id": "p1",
      "name": "Project 1",
      "slug": "project-1",
      "isFolder": false
    }
  ],
  "technology": {
    "id": "tech1",
    "icon": "icon.png",
    "name": "Tech 1",
    "description": "Tech description",
    "categories": [
      {
        "id": "cat1",
        "name": "Category 1"
      }
    ]
  },
  "cloudAccount": {
    "id": "acc1",
    "name": "Account 1",
    "cloudProvider": "AWS",
    "externalId": "123"
  },
  "issueAnalytics": {
    "issueCount": 10,
    "informationalSeverityCount": 1,
    "lowSeverityCount": 2,
    "mediumSeverityCount": 3,
    "highSeverityCount": 4,
    "criticalSeverityCount": 0
  }
}
""";
        JsonNode json = JsonNode.Parse(jsonString)!;
        WizUser user = WizUser.FromJson(json);

        Assert.AreEqual("1", user.Id);
        Assert.AreEqual("John Doe", user.Name);
        Assert.AreEqual(WizUserType.USER_ACCOUNT, user.Type);
        Assert.AreEqual(WizNativeType.AADUser, user.NativeType);
        Assert.AreEqual(DateTime.Parse("2024-05-01T00:00:00Z"), user.DeletedAt);

        Assert.IsTrue(user.HasAccessToSensitiveData);
        Assert.IsFalse(user.HasAdminPrivileges);
        Assert.IsTrue(user.HasHighPrivileges);
        Assert.IsTrue(user.HasSensitiveData);

        Assert.AreEqual("ge1", user.GraphEntityId);
        Assert.AreEqual(WizGraphEntityType.USER, user.GraphEntityType);
        Assert.AreEqual("value1", user.GraphEntityProperties["prop1"]);

        Assert.AreEqual(1, user.Projects.Count);
        WizProject project = user.Projects[0];
        Assert.AreEqual("p1", project.Id);
        Assert.AreEqual("Project 1", project.Name);
        Assert.AreEqual("project-1", project.Slug);
        Assert.IsFalse(project.IsFolder);

        Assert.IsNotNull(user.Technology);
        Assert.AreEqual("tech1", user.Technology!.Id);
        Assert.AreEqual("icon.png", user.Technology!.Icon);
        Assert.AreEqual("Tech 1", user.Technology!.Name);
        Assert.AreEqual("Tech description", user.Technology!.Description);
        Assert.AreEqual(1, user.Technology!.Categories.Count);
        Assert.AreEqual("cat1", user.Technology!.Categories[0].Id);
        Assert.AreEqual("Category 1", user.Technology!.Categories[0].Name);

        Assert.IsNotNull(user.CloudAccount);
        Assert.AreEqual("acc1", user.CloudAccount!.Id);
        Assert.AreEqual("Account 1", user.CloudAccount!.Name);
        Assert.AreEqual("AWS", user.CloudAccount!.CloudProvider);
        Assert.AreEqual("123", user.CloudAccount!.ExternalId);

        Assert.IsNotNull(user.IssueAnalytics);
        Assert.AreEqual(10, user.IssueAnalytics!.IssueCount);
        Assert.AreEqual(1, user.IssueAnalytics!.InformationalSeverityCount);
        Assert.AreEqual(2, user.IssueAnalytics!.LowSeverityCount);
        Assert.AreEqual(3, user.IssueAnalytics!.MediumSeverityCount);
        Assert.AreEqual(4, user.IssueAnalytics!.HighSeverityCount);
        Assert.AreEqual(0, user.IssueAnalytics!.CriticalSeverityCount);
    }

    [TestMethod]
    public void FromJson_IncompleteJson_DoesNotThrow() {
        string jsonString = """
        {
          "id": "2",
          "name": "Jane",
          "graphEntity": "invalid",
          "projects": ["bad"],
          "technology": 5,
          "issueAnalytics": true
        }
        """;

        JsonNode json = JsonNode.Parse(jsonString)!;

        WizUser user = WizUser.FromJson(json);

        Assert.AreEqual("2", user.Id);
        Assert.AreEqual("Jane", user.Name);
        Assert.AreEqual(0, user.Projects.Count);
        Assert.IsNull(user.Technology);
        Assert.IsNull(user.IssueAnalytics);
    }
}