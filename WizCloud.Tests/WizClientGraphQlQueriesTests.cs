using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizClientGraphQlQueriesTests {
    [TestMethod]
    public void WizClient_UsesGraphQlQueryConstants() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var directory = Path.Combine(repoRoot, "WizCloud");
        var source = string.Concat(Directory.GetFiles(directory, "WizClient*.cs").Select(File.ReadAllText));
        StringAssert.Contains(source, "GraphQlQueries.UsersQuery");
       StringAssert.Contains(source, "GraphQlQueries.ProjectsQuery");
       StringAssert.Contains(source, "GraphQlQueries.CloudAccountsQuery");
       StringAssert.Contains(source, "GraphQlQueries.IssuesQuery");
        StringAssert.Contains(source, "GraphQlQueries.VulnerabilitiesQuery");
        StringAssert.Contains(source, "GraphQlQueries.ResourcesQuery");
    }
}
