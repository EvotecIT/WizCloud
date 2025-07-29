using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace WizCloud.Tests;
[TestClass]
public sealed class GetWizUserTests {
    [TestMethod]
    public void ProgressRecordSetToCompleted() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud.PowerShell", "Cmdlets", "CmdletGetWizUser.cs");
        var source = File.ReadAllText(filePath);
        StringAssert.Contains(source, "RecordType = ProgressRecordType.Completed");
    }
}