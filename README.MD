# WizCloud - Modern Wiz.io Client for .NET and PowerShell

WizCloud is available as NuGet from the NuGet Gallery and as PowerShell module from PSGallery

📦 NuGet Package

[![nuget downloads](https://img.shields.io/nuget/dt/WizCloud?label=nuget%20downloads)](https://www.nuget.org/packages/WizCloud)
[![nuget version](https://img.shields.io/nuget/v/WizCloud)](https://www.nuget.org/packages/WizCloud)

💻 PowerShell Module

[![powershell gallery version](https://img.shields.io/powershellgallery/v/WizCloud.svg)](https://www.powershellgallery.com/packages/WizCloud)
[![powershell gallery preview](https://img.shields.io/powershellgallery/vpre/WizCloud.svg?label=powershell%20gallery%20preview&colorB=yellow)](https://www.powershellgallery.com/packages/WizCloud)
[![powershell gallery platforms](https://img.shields.io/powershellgallery/p/WizCloud.svg)](https://www.powershellgallery.com/packages/WizCloud)
[![powershell gallery downloads](https://img.shields.io/powershellgallery/dt/WizCloud.svg)](https://www.powershellgallery.com/packages/WizCloud)

🛠️ Project Information

[![top language](https://img.shields.io/github/languages/top/evotecit/WizCloud.svg)](https://github.com/EvotecIT/WizCloud)
[![license](https://img.shields.io/github/license/EvotecIT/WizCloud.svg)](https://github.com/EvotecIT/WizCloud)
[![codecov](https://codecov.io/gh/EvotecIT/WizCloud/branch/main/graph/badge.svg)](https://codecov.io/gh/EvotecIT/WizCloud)

👨‍💻 Author & Social

[![Twitter follow](https://img.shields.io/twitter/follow/PrzemyslawKlys.svg?label=Twitter%20%40PrzemyslawKlys&style=social)](https://twitter.com/PrzemyslawKlys)
[![Blog](https://img.shields.io/badge/Blog-evotec.xyz-2A6496.svg)](https://evotec.xyz/hub)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-pklys-0077B5.svg?logo=LinkedIn)](https://www.linkedin.com/in/pklys)
[![Threads](https://img.shields.io/badge/Threads-@PrzemyslawKlys-000000.svg?logo=Threads&logoColor=White)](https://www.threads.net/@przemyslaw.klys)
[![Discord](https://img.shields.io/discord/508328927853281280?style=flat-square&label=discord%20chat)](https://evo.yt/discord)

## What it's all about

**WizCloud** is an async C# library and PowerShell module for interacting with the Wiz.io GraphQL API. It provides a simple way to query cloud security data including users, projects, cloud accounts, and more. It supports multiple regions and provides both typed and raw data access. It's available for .NET 8, .NET Standard 2.0, and .NET 4.7.2.

## 🚀 Quick Start

### PowerShell
```powershell
# Install the module
Install-Module -Name WizCloud

# Connect to Wiz
Connect-Wiz -ClientId "your-client-id" -ClientSecret "your-secret" -Region EU17

# Get users
$users = Get-WizUser -MaxResults 100
$users | Where-Object { $_.Type -eq 'USER_ACCOUNT' } | Select-Object Name, Email, HasMfa

# Get cloud accounts
$accounts = Get-WizCloudAccount
$accounts | Group-Object CloudProvider | Select-Object Name, Count
```

### C#
```csharp
using WizCloud;

// Create client
var client = new WizClient(token, WizRegion.EU17);

// Get users
var users = await client.GetUsersAsync(pageSize: 100);
foreach (var user in users.Where(u => u.Type == WizUserType.USER_ACCOUNT)) {
    Console.WriteLine($"{user.Name} - MFA: {user.HasMfa}");
}

// Stream users (for large datasets)
await foreach (var user in client.GetUsersAsyncEnumerable(pageSize: 500)) {
    ProcessUser(user);
}
```

## 📊 Response Format Options: Raw vs Comprehensive Objects

WizCloud gives you **two ways** to work with API results, depending on your needs:

### 🔤 **Raw Objects (PowerShell with -Raw)**
Returns basic objects with GraphEntityProperties as a dictionary:
```powershell
$users = Get-WizUser -Raw
$users[0].GraphEntityProperties["userPrincipalName"]
```

### 🎯 **Comprehensive Objects (Default)**
Automatically expands all properties into **strongly-typed objects** with 73+ properties:
```powershell
$users = Get-WizUser
$users[0].UserPrincipalName     # Direct property access
$users[0].Department            # All properties exposed
$users[0].ProxyAddresses        # Complex properties parsed
$users[0].EmailAddresses        # Extracted from ProxyAddresses
```

**When to use each approach:**
- **Raw**: Direct API access, custom processing, smaller memory footprint
- **Comprehensive**: Full IntelliSense, easy filtering, all properties accessible

## 📋 Method/Cmdlet Comparison

| Operation | C# Method | PowerShell Cmdlet | Description |
|-----------|-----------|-------------------|-------------|
| **Authentication** | `new WizClient(token, region)` | `Connect-Wiz` | Authenticate with Wiz |
| | | `Disconnect-Wiz` | Clear stored credentials |
| **Users** | `GetUsersAsync()` | `Get-WizUser` | Get all users |
| | `GetUsersAsyncEnumerable()` | `Get-WizUser` | Stream users |
| **Projects** | `GetProjectsAsync()` | `Get-WizProject` | Get all projects |
| | `GetProjectsAsyncEnumerable()` | `Get-WizProject` | Stream projects |
| **Cloud Accounts** | `GetCloudAccountsAsync()` | `Get-WizCloudAccount` | Get cloud accounts |
| | `GetCloudAccountsAsyncEnumerable()` | `Get-WizCloudAccount` | Stream cloud accounts |

## 🔧 Installation

### PowerShell Module
```powershell
# Install from PowerShell Gallery
Install-Module -Name WizCloud -Force

# Import the module
Import-Module WizCloud
```

### NuGet Package
```bash
# Package Manager
Install-Package WizCloud

# .NET CLI
dotnet add package WizCloud

# PackageReference
<PackageReference Include="WizCloud" Version="1.0.0" />
```

## 💡 Examples

### PowerShell Examples

#### Connect and Get Users
```powershell
# Connect to Wiz
Connect-Wiz -ClientId $env:WIZ_CLIENT_ID -ClientSecret $env:WIZ_CLIENT_SECRET -Region EU17 -TestConnection

# Get all users with progress
$allUsers = Get-WizUser -Verbose

# Get specific user types
$serviceAccounts = Get-WizUser -Type SERVICE_ACCOUNT
$accessKeys = Get-WizUser -Type ACCESS_KEY

# Filter users without MFA
$noMfaUsers = Get-WizUser | Where-Object { $_.Type -eq 'USER_ACCOUNT' -and -not $_.HasMfa }

# Get users from specific project
$projectUsers = Get-WizUser -ProjectId "project-id"

# Export to CSV
Get-WizUser | Export-Csv -Path "WizUsers.csv" -NoTypeInformation
```

#### Work with Projects
```powershell
# Get all projects
$projects = Get-WizProject

# Find folder projects
$folders = $projects | Where-Object { $_.IsFolder }

# Get project hierarchy
$projects | Select-Object Name, Slug, IsFolder | Format-Table
```

#### Cloud Account Management
```powershell
# Get all cloud accounts
$accounts = Get-WizCloudAccount

# Group by provider
$accountsByProvider = $accounts | Group-Object CloudProvider
$accountsByProvider | ForEach-Object {
    Write-Host "$($_.Name): $($_.Count) accounts"
}

# Find AWS accounts
$awsAccounts = $accounts | Where-Object { $_.CloudProvider -eq 'AWS' }

# Find Azure subscriptions by name pattern
$devAccounts = $accounts | Where-Object { $_.Name -like '*DEV*' }
```

### C# Examples

#### Basic Usage
```csharp
using WizCloud;

// Create client with token refresh support
var client = new WizClient(token, WizRegion.US1, clientId, clientSecret);

// Get all users
var users = await client.GetUsersAsync(pageSize: 500);
Console.WriteLine($"Total users: {users.Count}");

// Filter by type
var userAccounts = users.Where(u => u.Type == WizUserType.USER_ACCOUNT);
var serviceAccounts = users.Where(u => u.Type == WizUserType.SERVICE_ACCOUNT);
```

#### Streaming Large Datasets
```csharp
// Stream users for memory efficiency
await foreach (var user in client.GetUsersAsyncEnumerable(pageSize: 1000)) {
    if (user.HasHighPrivileges) {
        Console.WriteLine($"High privilege user: {user.Name}");
    }
}

// Stream with cancellation
var cts = new CancellationTokenSource();
await foreach (var project in client.GetProjectsAsyncEnumerable(cancellationToken: cts.Token)) {
    ProcessProject(project);
    if (ShouldStop()) cts.Cancel();
}
```

#### Working with Comprehensive User Data
```csharp
// When using from C#, cast to WizUserComprehensive for all properties
var users = await client.GetUsersAsync();
foreach (var user in users) {
    // Access basic properties
    Console.WriteLine($"Name: {user.Name}");
    Console.WriteLine($"Type: {user.Type}");
    
    // Access GraphEntityProperties directly
    if (user.GraphEntityProperties.TryGetValue("department", out var dept)) {
        Console.WriteLine($"Department: {dept}");
    }
}
```

## 🌍 Supported Regions

| Region | Enum Value | API Endpoint |
|--------|------------|--------------|
| EU (Frankfurt) | `EU1` | `api.eu1.app.wiz.io` |
| EU (Belgium) | `EU2` | `api.eu2.app.wiz.io` |
| EU (London) | `EU17` | `api.eu17.app.wiz.io` |
| US East | `US1` | `api.us1.app.wiz.io` |
| US West | `US2` | `api.us2.app.wiz.io` |
| Australia | `AP1` | `api.ap1.app.wiz.io` |
| Japan | `AP2` | `api.ap2.app.wiz.io` |
| India | `AP3` | `api.ap3.app.wiz.io` |
| Singapore | `AP4` | `api.ap4.app.wiz.io` |
| US Gov | `GOV1` | `api.gov1.app.wiz.io` |

## 📊 User Types

| Type | Description | Common Properties |
|------|-------------|-------------------|
| `USER_ACCOUNT` | Regular user accounts | Email, MFA status, Department |
| `SERVICE_ACCOUNT` | Service/application accounts | ClientId, Managed status |
| `GROUP` | User groups | Member count |
| `ACCESS_KEY` | Access keys/credentials | ValidBefore, EverUsed, CredentialType |

## 🔒 Security Considerations

- Store credentials securely using environment variables or secure vaults
- Use service accounts with minimal required permissions
- Enable MFA for all user accounts
- Regularly rotate access keys
- Monitor API usage and rate limits

## 📈 Performance Tips

1. **Use Streaming for Large Datasets**
   ```powershell
   # Instead of loading all users into memory
   Get-WizUser | ForEach-Object { Process-User $_ }
   ```

2. **Specify MaxResults**
   ```powershell
   # Limit results when testing or when you need only a subset
   Get-WizUser -MaxResults 100
   ```

3. **Use Appropriate Page Sizes**
   ```powershell
   # Larger page sizes = fewer API calls
   Get-WizUser -PageSize 5000  # Max supported
   ```

4. **Filter at API Level**
   ```powershell
   # More efficient than client-side filtering
   Get-WizUser -Type SERVICE_ACCOUNT -ProjectId "project-id"
   ```

## 🐛 Troubleshooting

### Common Issues

1. **Authentication Errors**
   ```powershell
   # Ensure credentials are correct and have required permissions
   Connect-Wiz -ClientId "..." -ClientSecret "..." -TestConnection -Verbose
   ```

2. **Region Mismatch**
   ```powershell
   # Make sure you're connecting to the correct region
   Connect-Wiz -Region EU17  # Check your Wiz tenant region
   ```

3. **Rate Limiting**
   ```powershell
   # Add delays or reduce page size if hitting rate limits
   Get-WizUser -PageSize 100
   ```

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built on top of the Wiz.io GraphQL API
- Inspired by modern .NET practices and PowerShell standards
- Uses async/await patterns for optimal performance

## 📮 Contact

- **Blog**: [evotec.xyz](https://evotec.xyz)
- **Twitter**: [@PrzemyslawKlys](https://twitter.com/PrzemyslawKlys)
- **LinkedIn**: [pklys](https://www.linkedin.com/in/pklys)