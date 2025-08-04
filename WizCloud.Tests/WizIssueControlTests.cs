using System.Text.Json;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizIssueControlTests {
    [TestMethod]
    public void Deserialize_ParsesFields() {
        string jsonString = """
        {
          "id": "ctrl1",
          "name": "Control",
          "description": "desc",
          "severity": "HIGH"
        }
        """;
        WizIssueControl control = JsonSerializer.Deserialize<WizIssueControl>(jsonString, TestJson.Options)!;

        Assert.AreEqual("ctrl1", control.Id);
        Assert.AreEqual("Control", control.Name);
        Assert.AreEqual("desc", control.Description);
        Assert.AreEqual(WizSeverity.HIGH, control.Severity);
    }
}
