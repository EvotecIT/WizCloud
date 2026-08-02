---
external help file: WizCloud-help.xml
Module Name: WizCloud
online version: https://github.com/EvotecIT/WizCloud
schema: 2.0.0
---
# Get-WizUser
## SYNOPSIS
Gets users from Wiz.io.

## SYNTAX
### __AllParameterSets
```powershell
Get-WizUser [-PageSize <int>] [-Type <WizUserType[]>] [-ProjectId <string>] [-MaxResults <int>] [-Raw] [<CommonParameters>]
```

## DESCRIPTION
Retrieves user identities along with security properties and related projects.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Get-WizUser
```

Retrieves enhanced user objects for the current connection.

### EXAMPLE 2
```powershell
PS> Get-WizUser -MaxResults 10 -Raw
```

Outputs the first ten users using the raw API response.

## PARAMETERS

### -MaxResults
Maximum number of users to retrieve. Default is unlimited.

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
The number of users to retrieve per page.

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

### -Raw
Return raw API response objects.

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

### -Type
Filter by Wiz user types.

```yaml
Type: WizUserType[]
Parameter Sets: __AllParameterSets
Aliases: None
Possible values: USER_ACCOUNT, SERVICE_ACCOUNT, GROUP, ACCESS_KEY

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

- `WizCloud.WizUserComprehensive`
- `WizCloud.WizUser`

## RELATED LINKS

- [PowerShell documentation](https://learn.microsoft.com/powershell/scripting/overview)
- [Project documentation](https://github.com/EvotecIT/WizCloud)

## NOTES

### Note

Using -Raw returns original API objects and may consume additional memory.
