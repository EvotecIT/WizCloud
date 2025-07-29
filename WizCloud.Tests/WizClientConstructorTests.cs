using WizCloud.Enums;

namespace WizCloud.Tests {
    [TestClass]
    public sealed class WizClientConstructorTests {
        [TestMethod]
        public void Constructor_WithNullRegionString_ThrowsArgumentNullException() {
            Assert.ThrowsException<ArgumentNullException>(() => new WizClient("token", (string?)null));
        }

        [TestMethod]
        public void Constructor_WithNullRegionEnum_ThrowsArgumentNullException() {
            Assert.ThrowsException<ArgumentNullException>(() => new WizClient("token", (WizRegion?)null));
        }
    }
}