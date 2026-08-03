---
external help file: WizCloud-help.xml
Module Name: WizCloud
online version: https://github.com/EvotecIT/WizCloud
schema: 2.0.0
---
# Get-WizResource
## SYNOPSIS
Gets cloud resources from Wiz.io.

## SYNTAX
### __AllParameterSets
```powershell
Get-WizResource [-PageSize <int>] [-Type <string[]>] [-CloudProvider <WizCloudProvider[]>] [-Region <string>] [-PubliclyAccessible] [-Tag <hashtable>] [-ProjectId <string>] [-MaxResults <Int32>] [<CommonParameters>]
```

## DESCRIPTION
Retrieves resource inventory items from the Wiz API.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-WizResource
```

Returns all resources visible to the current connection.

### EXAMPLE 2
```powershell
PS> Get-WizResource -Type vm -PageSize 100
```

Retrieves virtual machines 100 at a time.

## PARAMETERS

### -CloudProvider
Filter by cloud provider.

```yaml
Type: WizCloudProvider[]
Parameter Sets: __AllParameterSets
Aliases: None
Possible values: AWS, AZURE, GCP, ALIBABA, OCI, KUBERNETES

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MaxResults
Maximum number of resources to retrieve. Default is unlimited.

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

### -PageSize
The number of resources to retrieve per page.

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

### -ProjectId
Filter by project identifier.

```yaml
Type: String
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -PubliclyAccessible
Filter by public accessibility.

```yaml
Type: SwitchParameter
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Region
Filter by resource region.

```yaml
Type: String
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Tag
Filter by tags.

```yaml
Type: Hashtable
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Type
Filter by resource type.

```yaml
Type: String[]
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

- `System.String`

## OUTPUTS

- `WizCloud.WizResource`

## RELATED LINKS

- [PowerShell documentation](https://learn.microsoft.com/powershell/scripting/overview)
- [Project documentation](https://github.com/EvotecIT/WizCloud)

## NOTES

### Note

Large result sets may require multiple API requests and increase run time.
