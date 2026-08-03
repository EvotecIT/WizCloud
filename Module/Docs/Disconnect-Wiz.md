---
external help file: WizCloud-help.xml
Module Name: WizCloud
online version: https://github.com/EvotecIT/WizCloud
schema: 2.0.0
---
# Disconnect-Wiz
## SYNOPSIS
Clears Wiz authentication from the current session.

## SYNTAX
### __AllParameterSets
```powershell
Disconnect-Wiz [<CommonParameters>]
```

## DESCRIPTION
Removes stored tokens, credentials, and region information for the session.

## EXAMPLES

### EXAMPLE 1
```powershell
PS> Disconnect-Wiz
```

This example clears any stored Wiz authentication information.

### EXAMPLE 2
```powershell
PS> Connect-Wiz -Token 'token'; Disconnect-Wiz
```

The connection created with Connect-Wiz is removed.

## PARAMETERS

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

Disconnecting deletes authentication details and cannot be undone for the current session.
