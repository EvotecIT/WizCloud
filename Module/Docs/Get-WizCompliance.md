---
external help file: WizCloud-help.xml
Module Name: WizCloud
online version: https://github.com/EvotecIT/WizCloud
schema: 2.0.0
---
# Get-WizCompliance
## SYNOPSIS
Gets compliance posture from Wiz.io.

## SYNTAX
### __AllParameterSets
```powershell
Get-WizCompliance [-Framework <string[]>] [-MinScore <Double>] [<CommonParameters>]
```

## DESCRIPTION
Retrieves compliance scores for supported frameworks.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-WizCompliance
```

Returns compliance scores for all available frameworks.

### EXAMPLE 2
```powershell
PS> Get-WizCompliance -Framework CIS -MinScore 80
```

Shows CIS results with scores of at least 80.

## PARAMETERS

### -Framework
Filter by compliance framework.

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

### -MinScore
Filter by minimum compliance score.

```yaml
Type: Double
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

- `WizCloud.WizComplianceResult`

## RELATED LINKS

- [PowerShell documentation](https://learn.microsoft.com/powershell/scripting/overview)
- [Project documentation](https://github.com/EvotecIT/WizCloud)

## NOTES

### Note

Results represent the latest assessment and may change as new scans run.
