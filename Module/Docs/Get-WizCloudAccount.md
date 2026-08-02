---
external help file: WizCloud-help.xml
Module Name: WizCloud
online version: https://github.com/EvotecIT/WizCloud
schema: 2.0.0
---
# Get-WizCloudAccount
## SYNOPSIS
Gets cloud accounts from Wiz.io.

## SYNTAX
### __AllParameterSets
```powershell
Get-WizCloudAccount [-PageSize <int>] [-MaxResults <int>] [<CommonParameters>]
```

## DESCRIPTION
Enumerates cloud provider accounts linked to your organization.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-WizCloudAccount
```

Lists every account accessible to the current connection.

### EXAMPLE 2
```powershell
PS> Get-WizCloudAccount -MaxResults 50 -PageSize 50
```

Retrieves at most fifty accounts in pages of fifty.

## PARAMETERS

### -MaxResults
Maximum number of cloud accounts to retrieve. Default is unlimited.

```yaml
Type: Nullable`1
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PageSize
The number of cloud accounts to retrieve per page.

```yaml
Type: Int32
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

- `None`

## OUTPUTS

- `WizCloud.WizCloudAccount`

## RELATED LINKS

- [PowerShell documentation](https://learn.microsoft.com/powershell/scripting/overview)
- [Project documentation](https://github.com/EvotecIT/WizCloud)

## NOTES

### Note

Retrieving many accounts may trigger API rate limiting.
