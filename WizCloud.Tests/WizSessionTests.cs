using WizCloud;

namespace WizCloud.Tests;

[TestClass]
[DoNotParallelize]
public sealed class WizSessionTests {
    [TestInitialize]
    public void Init() {
        WizSession.DefaultToken = null;
        WizSession.DefaultRegion = WizRegion.EU17;
    }

    [TestMethod]
    public void DefaultValues_AreExpected() {
        Assert.IsNull(WizSession.DefaultToken);
        Assert.AreEqual(WizRegion.EU17, WizSession.DefaultRegion);
    }

    [TestMethod]
    public void Properties_CanBeModified() {
        WizSession.DefaultToken = "token";
        WizSession.DefaultRegion = WizRegion.US1;

        Assert.AreEqual("token", WizSession.DefaultToken);
        Assert.AreEqual(WizRegion.US1, WizSession.DefaultRegion);
    }
}
