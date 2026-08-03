---
external help file: WizCloud-help.xml
Module Name: WizCloud
online version: https://github.com/EvotecIT/WizCloud
schema: 2.0.0
---
# Get-WizNetworkExposure
## SYNOPSIS
Gets network exposure data from Wiz.io.

## SYNTAX
### __AllParameterSets
```powershell
Get-WizNetworkExposure [-PageSize <int>] [-Port <int[]>] [-Protocol <string[]>] [-InternetFacing <Boolean>] [-ProjectId <string>] [-MaxResults <Int32>] [<CommonParameters>]
```

## DESCRIPTION
Retrieves open port and protocol exposure information.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-WizNetworkExposure
```

Returns every network exposure record.

### EXAMPLE 2
```powershell
PS> Get-WizNetworkExposure -Port 443 -Protocol tcp
```

Retrieves exposures for TCP port 443.

## PARAMETERS

### -InternetFacing
Filter by internet facing status.

```yaml
Type: Boolean
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -MaxResults
Maximum number of exposures to retrieve. Default is unlimited.

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
The number of exposures to retrieve per page.

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

### -Port
Filter by port.

```yaml
Type: Int32[]
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

### -Protocol
Filter by protocol.

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

- `WizCloud.WizNetworkExposure`

## RELATED LINKS

- [PowerShell documentation](https://learn.microsoft.com/powershell/scripting/overview)
- [Project documentation](https://github.com/EvotecIT/WizCloud)

## NOTES

### Note

Use port or protocol filters to limit the volume of returned data.
