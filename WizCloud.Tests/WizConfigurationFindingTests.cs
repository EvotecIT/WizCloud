using System;
using System.Text.Json;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizConfigurationFindingTests {
    [TestMethod]
    public void Deserialize_ParsesFields() {
        string jsonString = """
        {
            "id": "cf1",
            "title": "Sample",
            "description": "desc",
            "severity": "HIGH",
            "complianceFrameworks": ["CIS"],
            "failedResources": {
                "count": 1,
                "resources": [ { "id": "r1", "name": "res1", "type": "VM" } ]
            },
            "rule": { "id": "rule1", "name": "Rule 1", "category": "Network" },
            "remediation": "fix"
        }
        """;
        WizConfigurationFinding finding = JsonSerializer.Deserialize<WizConfigurationFinding>(jsonString, TestJson.Options)!;
        Assert.AreEqual("cf1", finding.Id);
        Assert.AreEqual("Sample", finding.Title);
        Assert.AreEqual(WizSeverity.HIGH, finding.Severity);
        Assert.AreEqual(1, finding.ComplianceFrameworks.Count);
        Assert.IsNotNull(finding.FailedResources);
        Assert.AreEqual(1, finding.FailedResources!.Count);
        Assert.AreEqual(1, finding.FailedResources!.Resources.Count);
        Assert.IsNotNull(finding.Rule);
    }
}
