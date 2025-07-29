using System;
using WizCloud.Enums;
using WizCloud.Helpers;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizRegionHelperTests
{
    [DataTestMethod]
    [DataRow(WizRegion.EU1, "eu1")]
    [DataRow(WizRegion.EU2, "eu2")]
    [DataRow(WizRegion.EU17, "eu17")]
    [DataRow(WizRegion.US1, "us1")]
    [DataRow(WizRegion.US2, "us2")]
    [DataRow(WizRegion.USGOV1, "usgov1")]
    [DataRow(WizRegion.AP1, "ap1")]
    [DataRow(WizRegion.AP2, "ap2")]
    [DataRow(WizRegion.CA1, "ca1")]
    public void ToApiString_ReturnsExpectedValue(WizRegion region, string expected)
    {
        string result = WizRegionHelper.ToApiString(region);
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow("eu1", WizRegion.EU1)]
    [DataRow("EU2", WizRegion.EU2)]
    [DataRow("Eu17", WizRegion.EU17)]
    [DataRow("us1", WizRegion.US1)]
    [DataRow("US2", WizRegion.US2)]
    [DataRow("USGOV1", WizRegion.USGOV1)]
    [DataRow("ap1", WizRegion.AP1)]
    [DataRow("Ap2", WizRegion.AP2)]
    [DataRow("CA1", WizRegion.CA1)]
    public void FromString_ReturnsExpectedEnum(string region, WizRegion expected)
    {
        WizRegion result = WizRegionHelper.FromString(region);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void FromString_InvalidValue_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() => WizRegionHelper.FromString("invalid"));
    }

    [TestMethod]
    public void ToApiString_InvalidEnum_Throws()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => WizRegionHelper.ToApiString((WizRegion)999));
    }
}
