using System.Text.Json;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizResourceIssueCountsTests {
    [TestMethod]
    public void Deserialize_ParsesCounts() {
        string jsonString = """
        {
          "criticalCount": 1,
          "highCount": 2,
          "mediumCount": 3,
          "lowCount": 4
        }
        """;
        WizResourceIssueCounts counts = JsonSerializer.Deserialize<WizResourceIssueCounts>(jsonString, TestJson.Options)!;

        Assert.AreEqual(1, counts.CriticalCount);
        Assert.AreEqual(2, counts.HighCount);
        Assert.AreEqual(3, counts.MediumCount);
        Assert.AreEqual(4, counts.LowCount);
    }
}
