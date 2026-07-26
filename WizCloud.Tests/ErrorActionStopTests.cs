using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace WizCloud.Tests;

[TestClass]
public sealed class ErrorActionStopTests {
    [TestMethod]
    public void AsyncCmdlets_DoNotRewrapActionPreferenceStopExceptions() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var cmdletDirectory = Path.Combine(repoRoot, "WizCloud.PowerShell", "Cmdlets");
        var unguardedCatches = Directory
            .EnumerateFiles(cmdletDirectory, "*.cs")
            .SelectMany(file => File.ReadLines(file).Select(line => (File: file, Line: line.Trim())))
            .Where(entry => entry.Line == "} catch (Exception ex) {")
            .Select(entry => Path.GetFileName(entry.File))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        Assert.AreEqual(
            0,
            unguardedCatches.Length,
            "Broad cmdlet catches must let ActionPreferenceStopException escape: " +
            string.Join(", ", unguardedCatches));
    }
}
