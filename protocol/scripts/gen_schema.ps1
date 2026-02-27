#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generate C# model classes from schema.json

.DESCRIPTION
    Generates strongly-typed C# models from the ACP JSON schema using Pydantic-style records.
    This script:
    1. Parses schema.json
    2. Generates C# records with System.Text.Json serialization
    3. Applies naming conventions (snake_case -> PascalCase)
    4. Handles type conversions and schema references
    5. Writes output to protocol/Schema.cs
#>

$ErrorActionPreference = "Stop"

# ============================================================================
# Helper Functions (defined first)
# ============================================================================

function Get-PropertyType {
    param(
        [PSObject]$Property,
        [PSObject]$AllDefinitions
    )

    # Handle $ref first
    if ($Property.'$ref') {
        $refName = $Property.'$ref'.Split('/')[-1]
        return Convert-NameToClass $refName
    }

    # Handle allOf with references
    if ($Property.allOf) {
        foreach ($allOfItem in $Property.allOf) {
            if ($allOfItem.'$ref') {
                $refName = $allOfItem.'$ref'.Split('/')[-1]
                return Convert-NameToClass $refName
            }
        }
    }

    # Handle array type
    if ($Property.type -eq "array") {
        $itemType = if ($Property.items) {
            Get-PropertyType $Property.items $AllDefinitions
        } else {
            "object"
        }
        return "List<$itemType>"
    }

    # Handle type arrays (e.g., ["string", "null"])
    $type = $Property.type
    $isNullable = $false

    if ($type -is [array]) {
        $isNullable = $null -in $type
        $type = @($type | Where-Object { $_ -ne $null })
        if ($type.Count -eq 1) {
            $type = $type[0]
        }
    }

    # Handle enum
    if ($Property.enum) {
        return "string"
    }

    # Handle typed values
    if ($type -is [string]) {
        $mappedType = Get-TypeName $type
        # Only add ? for value types (int, bool, double), not reference types
        if ($isNullable -and -not $mappedType.EndsWith('?')) {
            if ($mappedType -in @('int', 'bool', 'double')) {
                return "$mappedType?"
            }
        }
        return $mappedType
    }

    # Handle anyOf (union types) - return object without nullable annotation
    if ($Property.anyOf) {
        return "object"
    }

    # Handle oneOf (discriminated unions) - return object without nullable annotation
    if ($Property.oneOf) {
        return "object"
    }

    # Default to object (no nullable annotation)
    return "object"
}

function Get-TypeName {
    param([string]$JsonType)

    switch ($JsonType) {
        "string" { return "string" }
        "number" { return "double" }
        "integer" { return "int" }
        "boolean" { return "bool" }
        "object" { return "Dictionary<string, object>" }
        "array" { return "List<object>" }
        "null" { return "object" }
        default { return "object" }
    }
}

function Convert-PropertyName {
    param([string]$Name)

    # Convert snake_case to PascalCase
    # Special handling for _meta -> Meta
    if ($Name -eq "_meta") {
        return "Meta"
    }

    $parts = $Name -split '_'
    $result = ($parts | ForEach-Object { 
        if ($_.Length -gt 0) {
            # Keep existing casing for parts that are already PascalCase/camelCase
            # Just capitalize first letter
            $_.Substring(0, 1).ToUpper() + $_.Substring(1)
        }
    }) -join ''

    return $result
}

function Convert-NameToClass {
    param([string]$Name)

    # Ensure first letter is uppercase (PascalCase)
    if ($Name.Length -eq 0) { return $Name }
    return $Name.Substring(0, 1).ToUpper() + $Name.Substring(1)
}

function Convert-DefaultValue {
    param(
        $Value,
        [string]$Type
    )

    if ($null -eq $Value) {
        return $null
    }

    switch ($Type) {
        { $_ -match '^bool\??$' } { 
            if ($Value) { return "true" } else { return "false" }
        }
        { $_ -match '^int\??$' } { 
            return [int]$Value
        }
        { $_ -match '^double\??$' } { 
            return [double]$Value
        }
        { $_ -match '^string\??$' } { 
            return "`"$Value`""
        }
        { $_ -match '^List<' } {
            # C# 8.0 compatible: use full type syntax instead of new()
            return "new $Type()"
        }
        { $_ -match '^Dictionary<' } {
            # C# 8.0 compatible: use full type syntax instead of new()
            return "new $Type()"
        }
        default { 
            return $null 
        }
    }
}

function New-ModelClass {
    param(
        [string]$Name,
        [PSObject]$Definition,
        [PSObject]$AllDefinitions
    )

    $className = Convert-NameToClass $Name

    # Handle oneOf/anyOf at root level (union types or enums)
    if ($Definition.oneOf -or $Definition.anyOf) {
        $items = @(if ($Definition.oneOf) { $Definition.oneOf } else { $Definition.anyOf })

        # Check if this is an enum-like oneOf (all items have const)
        $isEnumLike = $items | Where-Object { $_.type -eq "string" -and $_.const } | Measure-Object | Select-Object -ExpandProperty Count

        if ($isEnumLike -eq $items.Count) {
            # This is an enum-like type, create a proper enum
            $xmlDocs = @()
            if ($Definition.description) {
                $xmlDocs += "/// <summary>"
                $description = $Definition.description -replace "`r`n", "`n"
                $descLines = $description -split "`n"
                foreach ($descLine in $descLines) {
                    $trimmedLine = $descLine.Trim()
                    if ($trimmedLine.Length -gt 0) {
                        $xmlDocs += "/// $trimmedLine"
                    } else {
                        $xmlDocs += "///"
                    }
                }
                $xmlDocs += "/// </summary>"
            }

            $result = $xmlDocs -join "`n"
            if ($xmlDocs.Count -gt 0) { $result += "`n" }

            $result += "[JsonConverter(typeof(JsonEnumMemberConverter<$className>))]`n"
            $result += "public enum $className`n{"

            # Generate enum values
            $enumValues = @()
            foreach ($item in $items) {
                $constValue = $item.const
                $enumName = Convert-PropertyName $constValue

                # Build enum value with description
                $enumValueDocs = @()
                if ($item.description) {
                    $enumValueDocs += "    /// <summary>"
                    $itemDesc = $item.description -replace "`r`n", "`n"
                    $itemDescLines = $itemDesc -split "`n"
                    foreach ($itemDescLine in $itemDescLines) {
                        $trimmedItemDescLine = $itemDescLine.Trim()
                        if ($trimmedItemDescLine.Length -gt 0) {
                            $enumValueDocs += "    /// $trimmedItemDescLine"
                        }
                    }
                    $enumValueDocs += "    /// </summary>"
                }

                $enumEntry = ""
                if ($enumValueDocs.Count -gt 0) {
                    $enumEntry += ($enumValueDocs -join "`n") + "`n"
                }

                # Add JsonEnumValue attribute if the enum name differs from the const value
                if ($enumName -cne $constValue) {
                    $enumEntry += "    [JsonEnumValue(`"$constValue`")]`n"
                }

                $enumEntry += "    $enumName"
                $enumValues += $enumEntry
            }

            $result += "`n"
            $result += ($enumValues -join ",`n") + "`n"
            $result += "}"

            return $result
        } else {
            # This is a union type - try to extract properties from union items
            $mergedProperties = @{}
            foreach ($item in $items) {
                if ($item.properties) {
                    foreach ($propName in $item.properties.PSObject.Properties.Name) {
                        if (-not $mergedProperties.ContainsKey($propName)) {
                            $mergedProperties[$propName] = $item.properties.$propName
                        }
                    }
                }
            }

            # If we found properties in the union items, use them
            if ($mergedProperties.Count -gt 0) {
                # Inject the merged properties into Definition for property processing below
                $Definition | Add-Member -MemberType NoteProperty -Name "properties" -Value ([PSCustomObject]$mergedProperties) -Force
                # Continue to normal property processing
            } else {
                # No properties found in union items, return empty record
                $xmlDocs = @()
                if ($Definition.description) {
                    $xmlDocs += "/// <summary>"
                    $description = $Definition.description -replace "`r`n", "`n"
                    $descLines = $description -split "`n"
                    foreach ($descLine in $descLines) {
                        $trimmedLine = $descLine.Trim()
                        if ($trimmedLine.Length -gt 0) {
                            $xmlDocs += "/// $trimmedLine"
                        } else {
                            $xmlDocs += "///"
                        }
                    }
                    $xmlDocs += "/// </summary>"
                }

                $result = $xmlDocs -join "`n"
                if ($xmlDocs.Count -gt 0) { $result += "`n" }
                $result += "public class $className { }"
                return $result
            }
        }
    }

    # Add description as XML comments if available
    $xmlDocs = @()
    if ($Definition.description) {
        $xmlDocs += "/// <summary>"
        # Handle multi-line descriptions - split by both \r\n and \n
        $description = $Definition.description -replace "`r`n", "`n"  # Normalize line endings
        $descLines = $description -split "`n"
        foreach ($descLine in $descLines) {
            $trimmedLine = $descLine.Trim()
            if ($trimmedLine.Length -gt 0) {
                $xmlDocs += "/// $trimmedLine"
            } else {
                # Preserve blank lines in multi-line descriptions
                $xmlDocs += "///"
            }
        }
        $xmlDocs += "/// </summary>"
    }

    # Determine class type
    $classDeclaration = "public class $className"

    # Build properties
    $properties = @()
    $props = $Definition.properties

    if ($null -ne $props -and $props.PSObject.Properties.Count -gt 0) {
        foreach ($propName in ($props.PSObject.Properties.Name | Sort-Object)) {
            $prop = $props.$propName
            $csType = Get-PropertyType $prop $AllDefinitions
            $csPropName = Convert-PropertyName $propName
            $propIsRequired = ($null -ne $Definition.required) -and ($Definition.required -contains $propName)
            $needsJsonPropertyName = $false

            # Handle naming conflicts: if property name matches class name, add suffix
            if ($csPropName -eq $className) {
                $csPropName = "{0}Value" -f $csPropName
                $needsJsonPropertyName = $true
            }

            # If not required and not nullable, make it nullable (only for value types)
            if (-not $propIsRequired -and -not $csType.EndsWith('?')) {
                # Only add ? for value types (int, bool, double), not reference types
                if ($csType -in @('int', 'bool', 'double')) {
                    $csType = "{0}?" -f $csType
                }
            }

            $propLine = ""

            # Add XML documentation for property if description exists
            if ($prop.description) {
                $propDocs = @()
                $propDocs += "    /// <summary>"
                $propDescription = $prop.description -replace "`r`n", "`n"
                $propDescLines = $propDescription -split "`n"
                foreach ($propDescLine in $propDescLines) {
                    $trimmedPropDescLine = $propDescLine.Trim()
                    if ($trimmedPropDescLine.Length -gt 0) {
                        $propDocs += "    /// $trimmedPropDescLine"
                    } else {
                        $propDocs += "    ///"
                    }
                }
                $propDocs += "    /// </summary>"
                $propLine = ($propDocs -join "`n") + "`n"
            }

            # Add JsonPropertyName attribute if name differs from property name
            # Or if we added a conflict suffix (renamed the property)
            if (-not $needsJsonPropertyName -and $propName -cne (Convert-PropertyName $propName)) {
                $needsJsonPropertyName = $true
            }

            if ($needsJsonPropertyName) {
                $propLine += "    [JsonPropertyName(`"$propName`")]`n"
            }

            # Build property declaration - use -f formatting instead of interpolation
            $propDeclaration = "    public {0} {1} {{ get; set; }}" -f $csType, $csPropName
            $propLine += $propDeclaration

            # Add default if specified
            if ($null -ne $prop.default) {
                $defaultValue = Convert-DefaultValue $prop.default $csType
                if ($defaultValue) {
                    $propLine = "{0} = {1};" -f $propLine, $defaultValue
                }
            }

            $properties += $propLine
        }
    }

    # Generate record declaration
    $result = $xmlDocs -join "`n"
    if ($xmlDocs.Count -gt 0) {
        $result += "`n"
    }

    if ($properties.Count -gt 0) {
        $result += "$classDeclaration`n{`n"
        $result += ($properties -join "`n") + "`n"
        $result += "}"
    } else {
        # Empty class - use traditional syntax for C# 8.0 compatibility
        $result += "$classDeclaration`n{`n}"
    }

    return $result
}

# ============================================================================
# Main Script
# ============================================================================

$ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProtocolDir = Split-Path -Parent $ScriptRoot
$RepoRoot = Split-Path -Parent $ProtocolDir
$SchemaDir = Join-Path $ProtocolDir "schema"
$SchemaJsonPath = Join-Path $SchemaDir "schema.json"
$VersionFile = Join-Path $SchemaDir "VERSION"
$OutputPath = Join-Path $ProtocolDir "Schema.cs"

if (-not (Test-Path $SchemaJsonPath)) {
    Write-Error "schema/schema.json not found" -ErrorAction Stop
}

Write-Host "  Parsing schema.json..." -ForegroundColor Gray
$schemaJson = Get-Content $SchemaJsonPath -Raw
$schemaContent = ConvertFrom-Json -InputObject $schemaJson

# Generate header
$headerLines = @()
$headerLines += "// Generated from schema/schema.json. Do not edit by hand."

if (Test-Path $VersionFile) {
    $ref = (Get-Content $VersionFile).Trim()
    if ($ref) {
        $headerLines += "// Schema ref: $ref"
    }
}

$header = $headerLines -join "`n"

# Generate using statement and namespace
$usings = @(
    "using System;"
    "using System.Collections.Generic;"
    "using System.Text.Json.Serialization;"
)

$output = @()
$output += $header
$output += ""
$output += $usings -join "`n"
$output += ""
$output += "namespace dotacp.protocol"
$output += "{"

# Generate classes from schema definitions
$defs = $schemaContent.'$defs'
if ($null -ne $defs) {
    $enumDefinitions = @()
    $recordClasses = @()

    foreach ($defName in ($defs.PSObject.Properties.Name | Sort-Object)) {
        $def = $defs.$defName
        $classCode = New-ModelClass $defName $def $defs

        # Check if this is an enum definition
        # Enums can have XML docs before the enum statement
        if ($classCode -match 'public\s+enum\s+\w+') {
            $enumDefinitions += $classCode
        } else {
            $recordClasses += $classCode
        }
    }

    # Add enums first
    if ($enumDefinitions.Count -gt 0) {
        $output += "    // Enums for string-based enum-like types"
        $output += ""
        foreach ($enum in $enumDefinitions) {
            # Indent enum definitions
            $indentedEnum = $enum -split "`n" | ForEach-Object { "    $_" }
            $output += $indentedEnum -join "`n"
            $output += ""
        }
    }

    # Then add class definitions
    $output += "    // Generated model classes from ACP schema"
    $output += ""
    foreach ($recordClass in $recordClasses) {
        # Indent class definitions
        $indentedClass = $recordClass -split "`n" | ForEach-Object { "    $_" }
        $output += $indentedClass -join "`n"
        $output += ""
    }
}

# Close namespace
$output += "}"

# Write output
$finalOutput = $output -join "`n"
if (-not $finalOutput.EndsWith("`n")) {
    $finalOutput += "`n"
}

Set-Content -Path $OutputPath -Value $finalOutput -Encoding UTF8 -NoNewline
$finalOutput | Out-Null  # Write with newline handled above
Add-Content -Path $OutputPath -Value "" -Encoding UTF8

Write-Host "  [OK] Generated C# models at $OutputPath" -ForegroundColor Gray
