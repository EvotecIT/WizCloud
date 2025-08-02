using System.Text.Json.Nodes;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizResourceIssueCountsTests {
    [TestMethod]
    public void FromJson_ParsesCounts() {
        string jsonString = """
        {
          "criticalCount": 1,
          "highCount": 2,
          "mediumCount": 3,
          "lowCount": 4
        }
        """;
        JsonNode json = JsonNode.Parse(jsonString)!;
        WizResourceIssueCounts counts = WizResourceIssueCounts.FromJson(json);

        Assert.AreEqual(1, counts.CriticalCount);
        Assert.AreEqual(2, counts.HighCount);
        Assert.AreEqual(3, counts.MediumCount);
        Assert.AreEqual(4, counts.LowCount);
    }
}
