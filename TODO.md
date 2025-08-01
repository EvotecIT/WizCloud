# Wiz PowerShell Module - TODO & Research

## Completed Features ✅

### 1. Get-WizUser
- Retrieves users including USER_ACCOUNT, SERVICE_ACCOUNT, GROUP, ACCESS_KEY
- Comprehensive object with 73+ typed properties
- ProxyAddresses parsing with email extraction
- ProjectNames property for easy access
- Supports filtering by Type and ProjectId
- Page size up to 5000, MaxResults parameter

### 2. Get-WizProject  
- Retrieves project hierarchy
- Properties: Id, Name, Slug, IsFolder
- Supports pagination and MaxResults

### 3. Get-WizCloudAccount
- Retrieves cloud accounts (AWS, Azure, GCP)
- Properties: Id, Name, CloudProvider, ExternalId
- Supports pagination and MaxResults

## Pending Features - Research 📋

### 4. Get-WizIssue (High Priority)
**Purpose**: Retrieve security issues and findings from Wiz

**Research Notes**:
- Issues are the main security findings in Wiz
- Include vulnerabilities, misconfigurations, exposed secrets, etc.
- Need to support filtering by severity, status, project, etc.

**Suggested GraphQL Query Structure**:
```graphql
query Issues($first: Int, $after: String, $filterBy: IssueFilters) {
    issues(first: $first, after: $after, filterBy: $filterBy) {
        pageInfo { hasNextPage endCursor }
        nodes {
            id
            name
            type
            severity
            status
            createdAt
            updatedAt
            resolvedAt
            dueAt
            projects { id name }
            resource {
                id
                name
                type
                cloudPlatform
                region
                subscriptionId
            }
            control {
                id
                name
                description
                severity
            }
            evidence
            remediation
        }
    }
}
```

**Properties to Include**:
- Id, Name, Type, Severity (CRITICAL, HIGH, MEDIUM, LOW, INFORMATIONAL)
- Status (OPEN, IN_PROGRESS, RESOLVED, REJECTED)
- Timestamps (CreatedAt, UpdatedAt, ResolvedAt, DueAt)
- Projects (array)
- Affected Resource details
- Control/Rule that triggered the issue
- Evidence and Remediation guidance

**Cmdlet Parameters**:
- `-Severity` (filter by severity levels)
- `-Status` (filter by issue status)
- `-ProjectId` (filter by project)
- `-Type` (filter by issue type)
- `-MaxResults`, `-PageSize`

### 5. Get-WizVulnerability (Medium Priority)
**Purpose**: Retrieve vulnerability findings specifically

**Research Notes**:
- Subset of issues focused on CVEs and software vulnerabilities
- Need CVE details, CVSS scores, affected packages
- Should include patch availability information

**Suggested GraphQL Query Structure**:
```graphql
query Vulnerabilities($first: Int, $after: String, $filterBy: VulnerabilityFilters) {
    vulnerabilities(first: $first, after: $after, filterBy: $filterBy) {
        pageInfo { hasNextPage endCursor }
        nodes {
            id
            cve
            cvss {
                score
                severity
                vector
            }
            publishedDate
            modifiedDate
            description
            affectedPackages {
                name
                version
                fixVersion
            }
            resources {
                id
                name
                type
                cloudPlatform
            }
            exploitAvailable
            exploitInTheWild
        }
    }
}
```

**Properties to Include**:
- CVE ID, CVSS Score/Severity
- Description, Published/Modified dates
- Affected packages with versions
- Fix versions available
- Exploit availability
- Affected resources

**Cmdlet Parameters**:
- `-CVE` (filter by specific CVE)
- `-MinCVSS` (minimum CVSS score)
- `-ExploitAvailable` (boolean filter)
- `-ProjectId`, `-MaxResults`, `-PageSize`

### 6. Get-WizResource (High Priority)
**Purpose**: Retrieve cloud resources (VMs, containers, databases, etc.)

**Research Notes**:
- Core inventory data for all cloud resources
- Should support filtering by type, cloud provider, tags
- Include security posture information

**Suggested GraphQL Query Structure**:
```graphql
query Resources($first: Int, $after: String, $filterBy: ResourceFilters) {
    resources(first: $first, after: $after, filterBy: $filterBy) {
        pageInfo { hasNextPage endCursor }
        nodes {
            id
            name
            type
            nativeType
            cloudPlatform
            cloudAccount { id name }
            region
            tags
            createdAt
            status
            publiclyAccessible
            hasPublicIpAddress
            isInternetFacing
            securityGroups
            issues {
                criticalCount
                highCount
                mediumCount
                lowCount
            }
        }
    }
}
```

**Properties to Include**:
- Basic info (Id, Name, Type, NativeType)
- Cloud details (Platform, Account, Region)
- Tags (key-value pairs)
- Network exposure status
- Security group assignments
- Issue summary counts

**Cmdlet Parameters**:
- `-Type` (VM, Container, Database, etc.)
- `-CloudProvider` (AWS, Azure, GCP)
- `-Region`
- `-PubliclyAccessible` (boolean)
- `-Tag` (hashtable for tag filtering)
- `-ProjectId`, `-MaxResults`, `-PageSize`

### 7. Get-WizConfigurationFinding (Medium Priority)
**Purpose**: Retrieve cloud configuration assessments

**Research Notes**:
- Configuration compliance findings
- Best practice violations
- CIS benchmark results

**Suggested GraphQL Query Structure**:
```graphql
query ConfigurationFindings($first: Int, $after: String, $filterBy: ConfigFindingFilters) {
    configurationFindings(first: $first, after: $after, filterBy: $filterBy) {
        pageInfo { hasNextPage endCursor }
        nodes {
            id
            title
            description
            severity
            complianceFrameworks
            failedResources {
                count
                resources { id name type }
            }
            rule {
                id
                name
                category
            }
            remediation
        }
    }
}
```

**Properties to Include**:
- Finding details (Id, Title, Description)
- Severity level
- Compliance frameworks (CIS, PCI-DSS, etc.)
- Failed resources count and list
- Rule/Control information
- Remediation guidance

**Cmdlet Parameters**:
- `-Framework` (CIS, PCI-DSS, HIPAA, etc.)
- `-Severity`
- `-Category` (IAM, Network, Storage, etc.)
- `-ProjectId`, `-MaxResults`, `-PageSize`

### 8. Get-WizNetworkExposure (Medium Priority)
**Purpose**: Retrieve internet-facing resources and exposure analysis

**Research Notes**:
- Resources exposed to the internet
- Open ports and services
- Attack surface analysis

**Suggested GraphQL Query Structure**:
```graphql
query NetworkExposure($first: Int, $after: String, $filterBy: ExposureFilters) {
    networkExposure(first: $first, after: $after, filterBy: $filterBy) {
        pageInfo { hasNextPage endCursor }
        nodes {
            id
            resource {
                id
                name
                type
            }
            exposureType
            ports
            protocols
            sourceIpRanges
            internetFacing
            publicIpAddress
            dnsName
            certificate {
                issuer
                expiryDate
                isValid
            }
        }
    }
}
```

**Properties to Include**:
- Resource information
- Exposure type (Direct, LoadBalancer, etc.)
- Open ports and protocols
- Source IP ranges allowed
- Public IP and DNS information
- SSL certificate details

**Cmdlet Parameters**:
- `-Port` (filter by specific port)
- `-Protocol` (TCP, UDP, etc.)
- `-InternetFacing` (boolean)
- `-ProjectId`, `-MaxResults`, `-PageSize`

### 9. Get-WizAuditLog (Low Priority)
**Purpose**: Retrieve audit logs for security activity tracking

**Research Notes**:
- User and system activities
- Configuration changes
- Access logs

**Suggested GraphQL Query Structure**:
```graphql
query AuditLogs($first: Int, $after: String, $filterBy: AuditLogFilters) {
    auditLogs(first: $first, after: $after, filterBy: $filterBy) {
        pageInfo { hasNextPage endCursor }
        nodes {
            id
            timestamp
            user {
                id
                name
                email
            }
            action
            resource {
                type
                id
                name
            }
            status
            ipAddress
            userAgent
            details
        }
    }
}
```

**Properties to Include**:
- Timestamp, User information
- Action performed
- Resource affected
- Success/Failure status
- Source IP and User Agent
- Additional details

**Cmdlet Parameters**:
- `-StartDate`, `-EndDate`
- `-User` (filter by user)
- `-Action` (filter by action type)
- `-Status` (Success, Failure)
- `-MaxResults`, `-PageSize`

### 10. Get-WizCompliance (Low Priority)
**Purpose**: Retrieve compliance posture and framework assessments

**Research Notes**:
- Overall compliance scores
- Framework-specific assessments
- Control pass/fail status

**Suggested GraphQL Query Structure**:
```graphql
query CompliancePosture($frameworks: [String!]) {
    compliancePosture(frameworks: $frameworks) {
        framework
        overallScore
        controls {
            id
            name
            status
            severity
            failedResourceCount
            passedResourceCount
        }
        lastAssessmentDate
    }
}
```

**Properties to Include**:
- Framework name and overall score
- Control-level details
- Pass/fail resource counts
- Assessment dates

**Cmdlet Parameters**:
- `-Framework` (array of frameworks)
- `-MinScore` (minimum compliance score)

## Implementation Guidelines

### Consistent Patterns to Follow:
1. **Cmdlet Structure**:
   - Use AsyncPSCmdlet base class
   - Implement proper cancellation support
   - Progress reporting for large datasets
   - Consistent parameter names (MaxResults, PageSize)

2. **Model Classes**:
   - Create comprehensive typed classes
   - Include FromJson static method
   - Consider creating "enhanced" versions for complex objects
   - Add convenience properties (like ProjectNames)

3. **WizClient Methods**:
   - Implement both GetXxxAsync and GetXxxAsyncEnumerable
   - Private GetXxxPageAsync for pagination
   - Consistent error handling
   - Use SendWithRefreshAsync for token refresh

4. **Error Handling**:
   - Specific error categories
   - Helpful error messages
   - Handle API rate limits gracefully

5. **Documentation**:
   - XML documentation on all public members
   - PowerShell help examples
   - Parameter descriptions

### Testing Requirements:
- Unit tests for model classes
- Integration tests for cmdlets
- Example scripts demonstrating usage
- Performance testing with large datasets

### Additional Considerations:
- Some queries might need to be combined (e.g., Issues might include vulnerabilities)
- Consider adding -Raw parameter for original API responses
- Add filtering capabilities at the API level where possible
- Consider caching for expensive queries (like compliance posture)
- Rate limiting awareness and retry logic

## Next Steps:
1. Validate GraphQL queries against actual Wiz API
2. Implement high-priority cmdlets first (Issues, Resources)
3. Create comprehensive test suite
4. Add telemetry/logging support
5. Performance optimization for large datasets
6. Consider batch operations for updates