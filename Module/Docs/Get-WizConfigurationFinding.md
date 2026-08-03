---
external help file: WizCloud-help.xml
Module Name: WizCloud
online version: https://github.com/EvotecIT/WizCloud
schema: 2.0.0
---
# Get-WizConfigurationFinding
## SYNOPSIS
Gets configuration findings from Wiz.io.

## SYNTAX
### __AllParameterSets
```powershell
Get-WizConfigurationFinding [-PageSize <int>] [-Framework <string[]>] [-Severity <WizSeverity[]>] [-Category <string[]>] [-ProjectId <string>] [-MaxResults <Int32>] [<CommonParameters>]
```

## DESCRIPTION
Retrieves configuration assessment results from the Wiz platform.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-WizConfigurationFinding
```

Returns all available configuration findings.

### EXAMPLE 2
```powershell
PS> Get-WizConfigurationFinding -Framework CIS
```

Retrieves findings related to the CIS framework.

## PARAMETERS

### -Category
Filter by category.

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

### -MaxResults
Maximum number of findings to retrieve. Default is unlimited.

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
The number of findings to retrieve per page.

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

### -Severity
Filter by severity.

```yaml
Type: WizSeverity[]
Parameter Sets: __AllParameterSets
Aliases: None
Possible values: INFORMATIONAL, LOW, MEDIUM, HIGH, CRITICAL

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

- `WizCloud.WizConfigurationFinding`

## RELATED LINKS

- [PowerShell documentation](https://learn.microsoft.com/powershell/scripting/overview)
- [Project documentation](https://github.com/EvotecIT/WizCloud)

## NOTES

### Note

Use filters such as framework or severity to limit the amount of data returned.
