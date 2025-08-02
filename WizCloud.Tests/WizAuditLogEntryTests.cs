using System;
using System.Text.Json.Nodes;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizAuditLogEntryTests {
    [TestMethod]
    public void FromJson_ParsesFields() {
        string jsonString = """
        {
            "id": "a1",
            "timestamp": "2024-06-01T00:00:00Z",
            "user": { "id": "u1", "name": "John", "email": "john@example.com" },
            "action": "LOGIN",
            "resource": { "type": "VM", "id": "r1", "name": "res1" },
            "status": "Success",
            "ipAddress": "1.1.1.1",
            "userAgent": "agent",
            "details": "detail"
        }
        """;
        JsonNode json = JsonNode.Parse(jsonString)!;
        WizAuditLogEntry entry = WizAuditLogEntry.FromJson(json);
        Assert.AreEqual("a1", entry.Id);
        Assert.IsNotNull(entry.User);
        Assert.AreEqual("LOGIN", entry.Action);
        Assert.IsNotNull(entry.Resource);
        Assert.AreEqual("Success", entry.Status);
    }
}
