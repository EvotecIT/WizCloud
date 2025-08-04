using System;
using System.Text.Json;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizIssueTests {
    [TestMethod]
    public void Deserialize_ParsesNestedObjects() {
        string jsonString = """
        {
          "id": "iss1",
          "name": "Issue",
          "type": "VULNERABILITY",
          "severity": "MEDIUM",
          "status": "OPEN",
          "createdAt": "2024-05-01T00:00:00Z",
          "projects": [{ "id": "p1", "name": "Project" }],
          "resource": {
            "id": "res1",
            "name": "Resource",
            "type": "VM",
            "cloudPlatform": "AWS"
          },
          "control": {
            "id": "ctrl1",
            "name": "Control",
            "severity": "LOW"
          },
          "evidence": "evidence",
          "remediation": "fix"
        }
        """;
        WizIssue issue = JsonSerializer.Deserialize<WizIssue>(jsonString, TestJson.Options)!;

        Assert.AreEqual("iss1", issue.Id);
        Assert.AreEqual("Issue", issue.Name);
        Assert.AreEqual("VULNERABILITY", issue.Type);
        Assert.AreEqual(WizSeverity.MEDIUM, issue.Severity);
        Assert.AreEqual("OPEN", issue.Status);
        Assert.AreEqual(DateTime.Parse("2024-05-01T00:00:00Z").ToUniversalTime(), issue.CreatedAt);
        Assert.AreEqual(1, issue.Projects.Count);
        Assert.IsNotNull(issue.Resource);
        Assert.AreEqual("res1", issue.Resource!.Id);
        Assert.IsNotNull(issue.Control);
        Assert.AreEqual(WizSeverity.LOW, issue.Control!.Severity);
        Assert.AreEqual("evidence", issue.Evidence);
        Assert.AreEqual("fix", issue.Remediation);
    }
}
