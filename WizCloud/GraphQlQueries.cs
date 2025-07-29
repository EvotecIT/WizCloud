namespace WizCloud;
/// <summary>
/// Provides GraphQL queries used by <see cref="WizClient"/>.
/// </summary>
public static class GraphQlQueries {
    /// <summary>
    /// Query for retrieving cloud identity principals.
    /// </summary>
    public const string UsersQuery = @"query CloudIdentityPrincipals($first: Int, $after: String, $filterBy: CloudResourceV2Filters) {
            cloudResourcesV2(first: $first, after: $after, filterBy: $filterBy) {
                pageInfo { hasNextPage endCursor }
                nodes { ...PrincipalDetails }
            }
        }
        fragment PrincipalDetails on CloudResourceV2 {
            id name type nativeType deletedAt
            graphEntity { id type properties }
            hasAccessToSensitiveData hasAdminPrivileges hasHighPrivileges hasSensitiveData
            projects { id name slug isFolder }
            technology { id icon name categories { id name } description }
            cloudAccount { id name cloudProvider externalId }
            issueAnalytics {
                issueCount informationalSeverityCount lowSeverityCount
                mediumSeverityCount highSeverityCount criticalSeverityCount
            }
        }";

    /// <summary>
    /// Query for retrieving projects.
    /// </summary>
    public const string ProjectsQuery = @"query Projects($first: Int, $after: String) {
            projects(first: $first, after: $after) {
                pageInfo { hasNextPage endCursor }
                nodes { id name slug isFolder }
            }
        }";
}
