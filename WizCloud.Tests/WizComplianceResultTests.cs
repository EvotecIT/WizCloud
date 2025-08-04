using System;
using System.Text.Json;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizComplianceResultTests {
    [TestMethod]
    public void Deserialize_ParsesFields() {
        string jsonString = """
        {
            "framework": "CIS",
            "overallScore": 0.9,
            "controls": [
                { "id": "c1", "name": "Control", "status": "PASS", "severity": "LOW", "failedResourceCount": 0, "passedResourceCount": 1 }
            ],
            "lastAssessmentDate": "2024-05-01T00:00:00Z"
        }
        """;
        WizComplianceResult result = JsonSerializer.Deserialize<WizComplianceResult>(jsonString, TestJson.Options)!;
        Assert.AreEqual("CIS", result.Framework);
        Assert.AreEqual(0.9, result.OverallScore);
        Assert.AreEqual(1, result.Controls.Count);
        Assert.IsNotNull(result.LastAssessmentDate);
    }
}
