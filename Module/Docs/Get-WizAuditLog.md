---
external help file: WizCloud-help.xml
Module Name: WizCloud
online version: https://github.com/EvotecIT/WizCloud
schema: 2.0.0
---
# Get-WizAuditLog
## SYNOPSIS
Gets audit logs from Wiz.io.

## SYNTAX
### __AllParameterSets
```powershell
Get-WizAuditLog [-PageSize <int>] [-StartDate <DateTime>] [-EndDate <DateTime>] [-User <string>] [-Action <string>] [-Status <string>] [-MaxResults <Int32>] [<CommonParameters>]
```

## DESCRIPTION
Retrieves records of user actions and system events.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-WizAuditLog
```

Returns audit logs using default paging.

### EXAMPLE 2
```powershell
PS> Get-WizAuditLog -User admin@example.com -StartDate (Get-Date).AddDays(-1)
```

Retrieves actions for the specified user in the last day.

## PARAMETERS

### -Action
Filter by action.

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

### -EndDate
Filter by end date.

```yaml
Type: DateTime
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
Maximum number of audit logs to retrieve. Default is unlimited.

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
The number of audit logs to retrieve per page.

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

### -StartDate
Filter by start date.

```yaml
Type: DateTime
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Status
Filter by status.

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

### -User
Filter by user.

```yaml
Type: String
Parameter Sets: __AllParameterSets
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: True (ByValue, ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

- `System.String`

## OUTPUTS

- `WizCloud.WizAuditLogEntry`

## RELATED LINKS

- [PowerShell documentation](https://learn.microsoft.com/powershell/scripting/overview)
- [Project documentation](https://github.com/EvotecIT/WizCloud)

## NOTES

### Note

Audit logs may be extensive; apply date or user filters to narrow the output.
