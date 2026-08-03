---
external help file: WizCloud-help.xml
Module Name: WizCloud
online version: https://github.com/EvotecIT/WizCloud
schema: 2.0.0
---
# Get-WizProject
## SYNOPSIS
Gets projects from Wiz.io.

## SYNTAX
### __AllParameterSets
```powershell
Get-WizProject [-PageSize <int>] [-MaxResults <Int32>] [<CommonParameters>]
```

## DESCRIPTION
Retrieves project and folder information from the Wiz API.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-WizProject
```

Returns every project for the connected organization.

### EXAMPLE 2
```powershell
PS> Get-WizProject -PageSize 100
```

Retrieves projects in pages of one hundred.

## PARAMETERS

### -MaxResults
Maximum number of projects to retrieve. Default is unlimited.

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
The number of projects to retrieve per page.

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

- `WizCloud.WizProject`

## RELATED LINKS

- [PowerShell documentation](https://learn.microsoft.com/powershell/scripting/overview)
- [Project documentation](https://github.com/EvotecIT/WizCloud)

## NOTES

### Note

Projects are returned in pages and may include folder entries.
