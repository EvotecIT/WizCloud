---
external help file: WizCloud-help.xml
Module Name: WizCloud
online version: https://github.com/EvotecIT/WizCloud
schema: 2.0.0
---
# Get-WizIssue
## SYNOPSIS
Gets security issues from Wiz.io.

## SYNTAX
### __AllParameterSets
```powershell
Get-WizIssue [-PageSize <int>] [-Severity <WizSeverity[]>] [-Status <string[]>] [-ProjectId <string>] [-Type <string[]>] [-MaxResults <int>] [<CommonParameters>]
```

## DESCRIPTION
Streams security issues reported by the Wiz platform.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-WizIssue
```

Returns every issue available to the current connection.

### EXAMPLE 2
```powershell
PS> Get-WizIssue -Severity High
```

Retrieves only high-severity issues.

## PARAMETERS

### -MaxResults
Maximum number of issues to retrieve. Default is unlimited.

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
The number of issues to retrieve per page.

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
Filter by issue severities.

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

### -Status
Filter by issue status.

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

### -Type
Filter by issue types.

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

- `WizCloud.WizIssue`

## RELATED LINKS

- [PowerShell documentation](https://learn.microsoft.com/powershell/scripting/overview)
- [Project documentation](https://github.com/EvotecIT/WizCloud)

## NOTES

### Note

Retrieving all issues may produce a large volume of output and take considerable time.
