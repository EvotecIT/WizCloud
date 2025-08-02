using System;
using System.IO;

namespace WizCloud.Tests;

[TestClass]
public sealed class GetWizNetworkExposureTests {
    [TestMethod]
    public void ProgressRecordSetToCompleted() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud.PowerShell", "Cmdlets", "CmdletGetWizNetworkExposure.cs");
        var source = File.ReadAllText(filePath);
        StringAssert.Contains(source, "RecordType = ProgressRecordType.Completed");
    }
}
