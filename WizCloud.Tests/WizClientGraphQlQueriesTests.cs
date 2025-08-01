using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace WizCloud.Tests;

[TestClass]
public sealed class WizClientGraphQlQueriesTests {
    [TestMethod]
    public void WizClient_UsesGraphQlQueryConstants() {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var filePath = Path.Combine(repoRoot, "WizCloud", "WizClient.cs");
        var source = File.ReadAllText(filePath);
        StringAssert.Contains(source, "GraphQlQueries.UsersQuery");
        StringAssert.Contains(source, "GraphQlQueries.ProjectsQuery");
        StringAssert.Contains(source, "GraphQlQueries.CloudAccountsQuery");
        StringAssert.Contains(source, "GraphQlQueries.IssuesQuery");
    }
}
