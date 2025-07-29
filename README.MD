# WizCloud

WizCloud is a set of libraries and PowerShell cmdlets for interacting with the Wiz API.

## Projects

- **WizCloud** – cross‑platform client library targeting `netstandard2.0`, `net472` and `net8.0`.
- **WizCloud.PowerShell** – PowerShell module built on top of the client library.
- **WizCloud.Examples** – console samples demonstrating basic scenarios.
- **WizCloud.Tests** – MSTest unit tests.

## Building the solution

Use the .NET 8 SDK. Restore dependencies and build in Release mode:

```bash
 dotnet build WizCloud.sln -c Release
```

To run unit tests:

```bash
 dotnet test WizCloud.sln -c Release --no-build
```

## Sample usage

### C# library

```csharp
using WizCloud;

var token = Environment.GetEnvironmentVariable("WIZ_SERVICE_ACCOUNT_TOKEN");
using var client = new WizClient(token!, WizRegion.EU17);
var projects = await client.GetProjectsAsync();
Console.WriteLine($"Retrieved {projects.Count} projects");
```

### PowerShell module

```powershell
# Import the module from the repository
Import-Module ./Module/WizCloud.psd1 -Force

# Connect using an environment variable or provide a token
Connect-Wiz -TestConnection

# Retrieve users
Get-WizUser -PageSize 50 | Select-Object Name, Type
```
