using System;
using System.Text.Json.Nodes;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizConfigurationFindingTests {
    [TestMethod]
    public void FromJson_ParsesFields() {
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
        JsonNode json = JsonNode.Parse(jsonString)!;
        WizConfigurationFinding finding = WizConfigurationFinding.FromJson(json);
        Assert.AreEqual("cf1", finding.Id);
        Assert.AreEqual("Sample", finding.Title);
        Assert.AreEqual(WizSeverity.HIGH, finding.Severity);
        Assert.AreEqual(1, finding.ComplianceFrameworks.Count);
        Assert.AreEqual(1, finding.FailedResourceCount);
        Assert.IsNotNull(finding.Rule);
    }
}
