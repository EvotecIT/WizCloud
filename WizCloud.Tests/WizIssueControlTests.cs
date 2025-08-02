using System.Text.Json.Nodes;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizIssueControlTests {
    [TestMethod]
    public void FromJson_ParsesFields() {
        string jsonString = """
        {
          "id": "ctrl1",
          "name": "Control",
          "description": "desc",
          "severity": "HIGH"
        }
        """;
        JsonNode json = JsonNode.Parse(jsonString)!;
        WizIssueControl control = WizIssueControl.FromJson(json);

        Assert.AreEqual("ctrl1", control.Id);
        Assert.AreEqual("Control", control.Name);
        Assert.AreEqual("desc", control.Description);
        Assert.AreEqual(WizSeverity.HIGH, control.Severity);
    }
}
