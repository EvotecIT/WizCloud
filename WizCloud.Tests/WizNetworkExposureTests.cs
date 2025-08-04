using System;
using System.Text.Json;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizNetworkExposureTests {
    [TestMethod]
    public void Deserialize_ParsesFields() {
        string jsonString = """
        {
            "id": "ne1",
            "resource": { "id": "r1", "name": "res1", "type": "VM" },
            "exposureType": "Direct",
            "ports": [80],
            "protocols": ["TCP"],
            "sourceIpRanges": ["0.0.0.0/0"],
            "internetFacing": true,
            "publicIpAddress": "1.2.3.4",
            "dnsName": "example.com",
            "certificate": { "issuer": "CA", "expiryDate": "2024-01-01T00:00:00Z", "isValid": true }
        }
        """;
        WizNetworkExposure exposure = JsonSerializer.Deserialize<WizNetworkExposure>(jsonString, TestJson.Options)!;
        Assert.AreEqual("ne1", exposure.Id);
        Assert.IsNotNull(exposure.Resource);
        Assert.AreEqual("Direct", exposure.ExposureType);
        Assert.AreEqual(1, exposure.Ports.Count);
        Assert.IsTrue(exposure.InternetFacing);
        Assert.IsNotNull(exposure.Certificate);
    }
}
