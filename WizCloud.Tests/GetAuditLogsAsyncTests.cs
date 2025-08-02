using System.Collections.Generic;
using System.Threading.Tasks;
using WizCloud;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetAuditLogsAsyncTests {
    [TestMethod]
    public void MethodExistsWithCorrectReturnType() {
        var method = typeof(WizClient).GetMethod("GetAuditLogsAsync");
        Assert.IsNotNull(method);
        Assert.AreEqual(typeof(Task<List<WizAuditLogEntry>>), method!.ReturnType);
    }
}
