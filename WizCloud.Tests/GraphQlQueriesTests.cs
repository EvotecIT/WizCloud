using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace WizCloud.Tests;

[TestClass]
public sealed class GraphQlQueriesTests {
    [TestMethod]
    public void UsersQuery_ConstantExists() {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var field = typeof(GraphQlQueries).GetField("UsersQuery", flags);
        Assert.IsNotNull(field);
        Assert.AreEqual(typeof(string), field!.FieldType);
    }

    [TestMethod]
    public void ProjectsQuery_ConstantExists() {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var field = typeof(GraphQlQueries).GetField("ProjectsQuery", flags);
        Assert.IsNotNull(field);
        Assert.AreEqual(typeof(string), field!.FieldType);
    }

    [TestMethod]
    public void CloudAccountsQuery_ConstantExists() {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var field = typeof(GraphQlQueries).GetField("CloudAccountsQuery", flags);
        Assert.IsNotNull(field);
        Assert.AreEqual(typeof(string), field!.FieldType);
    }

    [TestMethod]
    public void IssuesQuery_ConstantExists() {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var field = typeof(GraphQlQueries).GetField("IssuesQuery", flags);
        Assert.IsNotNull(field);
        Assert.AreEqual(typeof(string), field!.FieldType);
    }

    [TestMethod]
    public void ResourcesQuery_ConstantExists() {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var field = typeof(GraphQlQueries).GetField("ResourcesQuery", flags);
        Assert.IsNotNull(field);
        Assert.AreEqual(typeof(string), field!.FieldType);
    }

    [TestMethod]
    public void VulnerabilitiesQuery_ConstantExists() {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var field = typeof(GraphQlQueries).GetField("VulnerabilitiesQuery", flags);
        Assert.IsNotNull(field);
        Assert.AreEqual(typeof(string), field!.FieldType);
    }

    [TestMethod]
    public void ConfigurationFindingsQuery_ConstantExists() {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var field = typeof(GraphQlQueries).GetField("ConfigurationFindingsQuery", flags);
        Assert.IsNotNull(field);
        Assert.AreEqual(typeof(string), field!.FieldType);
    }

    [TestMethod]
    public void NetworkExposureQuery_ConstantExists() {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var field = typeof(GraphQlQueries).GetField("NetworkExposureQuery", flags);
        Assert.IsNotNull(field);
        Assert.AreEqual(typeof(string), field!.FieldType);
    }

    [TestMethod]
    public void AuditLogsQuery_ConstantExists() {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var field = typeof(GraphQlQueries).GetField("AuditLogsQuery", flags);
        Assert.IsNotNull(field);
        Assert.AreEqual(typeof(string), field!.FieldType);
    }

    [TestMethod]
    public void CompliancePostureQuery_ConstantExists() {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
        var field = typeof(GraphQlQueries).GetField("CompliancePostureQuery", flags);
        Assert.IsNotNull(field);
        Assert.AreEqual(typeof(string), field!.FieldType);
    }
}
