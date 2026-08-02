---
external help file: WizCloud-help.xml
Module Name: WizCloud
online version: https://github.com/EvotecIT/WizCloud
schema: 2.0.0
---
# Connect-Wiz
## SYNOPSIS
Connects to Wiz.io and stores authentication for the session.

## SYNTAX
### TokenParameterSet
```powershell
Connect-Wiz [-Token] <string> [-Region <WizRegion>] [-TestConnection] [-Suppress] [<CommonParameters>]
```

### ClientCredentialParameterSet
```powershell
Connect-Wiz -ClientId <string> -ClientSecret <string> [-Region <WizRegion>] [-TestConnection] [-Suppress] [<CommonParameters>]
```

## DESCRIPTION
Establishes a connection using a token or client credentials and optionally tests the API.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Connect-Wiz -Token 'your-service-account-token'
```

The token is cached for subsequent cmdlets.

### EXAMPLE 2
```powershell
PS> Connect-Wiz -ClientId 'id' -ClientSecret 'secret' -Region us1
```

A token is acquired for region us1.

## PARAMETERS

### -ClientId
The Wiz service account client ID.

```yaml
Type: String
Parameter Sets: ClientCredentialParameterSet
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ClientSecret
The Wiz service account client secret.

```yaml
Type: String
Parameter Sets: ClientCredentialParameterSet
Aliases: None
Possible values:

Required: True
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Region
The Wiz region to connect to (e.g., 'eu17', 'us1', 'us2').

```yaml
Type: WizRegion
Parameter Sets: TokenParameterSet, ClientCredentialParameterSet
Aliases: None
Possible values: EU1, EU2, EU17, US1, US2, USGOV1, AP1, AP2, CA1

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Suppress
Suppress the output messages.

```yaml
Type: SwitchParameter
Parameter Sets: TokenParameterSet, ClientCredentialParameterSet
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -TestConnection
Test the connection to Wiz.

```yaml
Type: SwitchParameter
Parameter Sets: TokenParameterSet, ClientCredentialParameterSet
Aliases: None
Possible values:

Required: False
Position: named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Token
The Wiz service account token for authentication.

```yaml
Type: String
Parameter Sets: TokenParameterSet
Aliases: None
Possible values:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

- `None`

## OUTPUTS

- `System.Boolean`

## RELATED LINKS

- [PowerShell documentation](https://learn.microsoft.com/powershell/scripting/overview)
- [Project documentation](https://github.com/EvotecIT/WizCloud)

## NOTES

### Note

Stored credentials remain in memory until Disconnect-Wiz is executed.
