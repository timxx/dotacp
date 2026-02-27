#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generate C# model classes from schema.json

.DESCRIPTION
    Generates strongly-typed C# models from the ACP JSON schema using Pydantic-style records.
    This script:
    1. Parses schema.json
    2. Generates C# records with Newtonsoft.Json serialization
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

    # Handle type arrays (e.g., ["string", "null"], ["integer", "null"])
    $type = $Property.type
    $isNullable = $false

    if ($type -is [array]) {
        # Check for "null" string (as parsed by ConvertFrom-Json)
        $isNullable = "null" -in $type
        $type = @($type | Where-Object { $_ -ne "null" })
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
        # Check for format hints on integer types
        if ($type -eq "integer" -and $Property.format) {
            $mappedType = switch ($Property.format) {
                "uint16" { "ushort" }
                "uint32" { "uint" }
                "uint64" { "ulong" }
                "int16" { "short" }
                "int32" { "int" }
                "int64" { "long" }
                default { "int" }
            }
        } else {
            $mappedType = Get-TypeName $type
        }

        if ($isNullable -and $type -eq "object") {
            return "object"
        }

        # Add ? for nullable value types only
        if ($isNullable -and -not $mappedType.EndsWith('?')) {
            # Only add ? for value types (int, bool, double, uint, ushort, ulong, short, long, byte, sbyte, float, decimal), not reference types
            if ($mappedType -in @('int', 'bool', 'double', 'uint', 'ushort', 'ulong', 'short', 'long', 'byte', 'sbyte', 'float', 'decimal')) {
                return $mappedType + "?"
            }
        }
        return $mappedType
    }

    # Handle anyOf (union types)
    if ($Property.anyOf) {
        # Check if this is just a nullable reference pattern: anyOf: [{ $ref }, { type: null }]
        if ($Property.anyOf.Count -eq 2) {
            $hasRef = $false
            $hasNull = $false
            $refType = $null

            foreach ($item in $Property.anyOf) {
                if ($item.'$ref') {
                    $hasRef = $true
                    $refName = $item.'$ref'.Split('/')[-1]
                    $refType = Convert-NameToClass $refName
                }
                elseif ($item.type -eq "null") {
                    $hasNull = $true
                }
            }

            # If it's a ref + null pattern, return the nullable reference type
            if ($hasRef -and $hasNull -and $refType) {
                return $refType
            }
        }

        # Otherwise, return object for complex union types
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
        "object" { return "object" }
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

function Test-IsSimpleTypeAlias {
    param(
        [PSObject]$Definition
    )

    # A simple type alias is a definition that:
    # 1. Has a 'type' property that is a simple string (not an array)
    # 2. Has no properties, allOf, oneOf, anyOf, enum, etc.
    # 3. May have description, format, minimum, maximum (these are just documentation/validation)

    if (-not $Definition.type) {
        return $false
    }

    # type must be a simple string, not an array
    if ($Definition.type -is [array]) {
        return $false
    }

    # Must not have any complex schema properties
    $complexProperties = @('properties', 'allOf', 'oneOf', 'anyOf', 'enum', 'items', 'additionalProperties', 'required', 'discriminator')
    foreach ($prop in $complexProperties) {
        if ($null -ne $Definition.$prop) {
            return $false
        }
    }

    return $true
}

function Get-TypeAliasTarget {
    param(
        [PSObject]$Definition
    )

    $baseType = Get-TypeName $Definition.type

    # Check for format hints on integer types
    if ($Definition.type -eq "integer" -and $Definition.format) {
        switch ($Definition.format) {
            "uint16" { return "ushort" }
            "uint32" { return "uint" }
            "uint64" { return "ulong" }
            "int16" { return "short" }
            "int32" { return "int" }
            "int64" { return "long" }
            default { return "int" }
        }
    }

    return $baseType
}

function New-TypeAliasStruct {
    param(
        [string]$Name,
        [PSObject]$Definition,
        [string]$UnderlyingType
    )

    $className = Convert-NameToClass $Name

    # Build XML documentation
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

    # For C# 8.0 compatibility, generate a readonly struct with value property
    # and implicit conversions to/from the underlying type
    $result = $xmlDocs -join "`n"
    if ($xmlDocs.Count -gt 0) { $result += "`n" }

    $result += "[JsonConverter(typeof(TypeAliasConverter<$className, $UnderlyingType>))]`n"
    $result += "public readonly struct $className : IEquatable<$className>`n"
    $result += "{`n"
    $result += "    private readonly $UnderlyingType _value;`n"
    $result += "`n"
    $result += "    public $className($UnderlyingType value)`n"
    $result += "    {`n"
    $result += "        _value = value;`n"
    $result += "    }`n"
    $result += "`n"
    $result += "    public static implicit operator $className($UnderlyingType value) => new $className(value);`n"
    $result += "    public static implicit operator $UnderlyingType($className alias) => alias._value;`n"
    $result += "`n"

    # Handle GetHashCode and ToString differently for value types vs reference types
    $isValueType = $UnderlyingType -in @('ushort', 'uint', 'ulong', 'short', 'int', 'long', 'byte', 'sbyte', 'bool', 'double', 'float', 'decimal')

    if ($isValueType) {
        $result += "    public bool Equals($className other) => _value == other._value;`n"
        $result += "    public override bool Equals(object obj) => obj is $className other && Equals(other);`n"
        $result += "    public override int GetHashCode() => _value.GetHashCode();`n"
        $result += "    public override string ToString() => _value.ToString();`n"
    } else {
        $result += "    public bool Equals($className other) => _value == other._value;`n"
        $result += "    public override bool Equals(object obj) => obj is $className other && Equals(other);`n"
        $result += "    public override int GetHashCode() => _value?.GetHashCode() ?? 0;`n"
        $result += "    public override string ToString() => _value?.ToString() ?? string.Empty;`n"
    }

    $result += "}"

    return $result
}

function New-UnionTypeStruct {
    param(
        [string]$Name,
        [PSObject]$Definition,
        [array]$UnionTypes,
        [bool]$HasNullType = $false
    )

    $className = Convert-NameToClass $Name

    # Remove duplicate types (keep unique types only)
    $uniqueUnionTypes = $UnionTypes | Select-Object -Unique

    # Build XML documentation
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

    # Generate a readonly struct that can hold any of the union types
    $result = $xmlDocs -join "`n"
    if ($xmlDocs.Count -gt 0) { $result += "`n" }

    $result += "[JsonConverter(typeof(UnionTypeConverter<$className>))]`n"
    $result += "public readonly struct $className : IEquatable<$className>`n"
    $result += "{`n"
    $result += "    private readonly object _value;`n"
    $result += "    private readonly int _typeIndex;`n"

    if ($HasNullType) {
        $result += "    private readonly bool _isNull;`n"
    }

    $result += "`n"

    # Generate constructors for each unique type
    $ctorIndex = 0
    foreach ($unionType in $uniqueUnionTypes) {
        $result += "    public $className($unionType value)`n"
        $result += "    {`n"
        $result += "        _value = value;`n"
        $result += "        _typeIndex = $ctorIndex;`n"
        if ($HasNullType) {
            $result += "        _isNull = false;`n"
        }
        $result += "    }`n"
        $result += "`n"
        $ctorIndex++
    }

    # Add null constructor if needed
    if ($HasNullType) {
        $result += "    private $className(bool isNull)`n"
        $result += "    {`n"
        $result += "        _value = null;`n"
        $result += "        _typeIndex = -1;`n"
        $result += "        _isNull = isNull;`n"
        $result += "    }`n"
        $result += "`n"
        $result += "    public static $className Null => new $className(true);`n"
        $result += "`n"
    }

    # Generate implicit conversions from each unique type
    foreach ($unionType in $uniqueUnionTypes) {
        $result += "    public static implicit operator $className($unionType value) => new $className(value);`n"
    }
    $result += "`n"

    # Add null check property if needed
    if ($HasNullType) {
        $result += "    public bool IsNull => _isNull;`n"
        $result += "`n"
    }

    # Generate TryGet methods for each unique type
    foreach ($unionType in $uniqueUnionTypes) {
        # Handle generic types like List<T> - extract the type name properly
        $cleanTypeName = $unionType -replace '[<>,\s]', ''
        $methodName = "TryGet" + $cleanTypeName.Substring(0,1).ToUpper() + $cleanTypeName.Substring(1)

        $result += "    public bool $methodName(out $unionType value)`n"
        $result += "    {`n"
        if ($HasNullType) {
            $result += "        if (_isNull)`n"
            $result += "        {`n"
            $result += "            value = default;`n"
            $result += "            return false;`n"
            $result += "        }`n"
        }
        $result += "        if (_value is $unionType v)`n"
        $result += "        {`n"
        $result += "            value = v;`n"
        $result += "            return true;`n"
        $result += "        }`n"
        $result += "        value = default;`n"
        $result += "        return false;`n"
        $result += "    }`n"
        $result += "`n"
    }

    # Generate Equals, GetHashCode, ToString
    # Use manual hash code combination for .NET Framework 4.7.2 compatibility
    if ($HasNullType) {
        $result += "    public bool Equals($className other) => _isNull == other._isNull && (_isNull || (Equals(_value, other._value) && _typeIndex == other._typeIndex));`n"
        $result += "    public override bool Equals(object obj) => obj is $className other && Equals(other);`n"
        $result += "    public override int GetHashCode()`n"
        $result += "    {`n"
        $result += "        if (_isNull) return 0;`n"
        $result += "        unchecked`n"
        $result += "        {`n"
        $result += "            int hash = 17;`n"
        $result += "            hash = hash * 31 + (_value != null ? _value.GetHashCode() : 0);`n"
        $result += "            hash = hash * 31 + _typeIndex;`n"
        $result += "            return hash;`n"
        $result += "        }`n"
        $result += "    }`n"
        $result += "    public override string ToString() => _isNull ? string.Empty : (_value?.ToString() ?? string.Empty);`n"
    } else {
        $result += "    public bool Equals($className other) => Equals(_value, other._value) && _typeIndex == other._typeIndex;`n"
        $result += "    public override bool Equals(object obj) => obj is $className other && Equals(other);`n"
        $result += "    public override int GetHashCode()`n"
        $result += "    {`n"
        $result += "        unchecked`n"
        $result += "        {`n"
        $result += "            int hash = 17;`n"
        $result += "            hash = hash * 31 + (_value != null ? _value.GetHashCode() : 0);`n"
        $result += "            hash = hash * 31 + _typeIndex;`n"
        $result += "            return hash;`n"
        $result += "        }`n"
        $result += "    }`n"
        $result += "    public override string ToString() => _value?.ToString() ?? string.Empty;`n"
    }

    $result += "}"

    return $result
}

function New-ModelClass {
    param(
        [string]$Name,
        [PSObject]$Definition,
        [PSObject]$AllDefinitions
    )

    $className = Convert-NameToClass $Name

    # Check if this is a simple type alias
    if (Test-IsSimpleTypeAlias $Definition) {
        # Generate a type alias struct
        $targetType = Get-TypeAliasTarget $Definition
        return New-TypeAliasStruct $Name $Definition $targetType
    }

    # Handle simple enum definitions (e.g., { type: "string", enum: ["value1", "value2"] })
    if ($Definition.enum) {
        # Build XML documentation
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

        # Determine enum backing type based on enum values
        $enumType = $Definition.type
        if ($enumType -is [array]) {
            $enumType = ($enumType | Where-Object { $_ -ne "null" })[0]
        }

        if ($enumType -eq "integer") {
            # Check format for backing type
            $backingType = "int"
            if ($Definition.format) {
                switch ($Definition.format) {
                    "int64" { $backingType = "long" }
                    "uint16" { $backingType = "ushort" }
                    "uint32" { $backingType = "uint" }
                    "uint64" { $backingType = "ulong" }
                    "int16" { $backingType = "short" }
                    "int32" { $backingType = "int" }
                    default { $backingType = "int" }
                }
            }
            # Integer enums don't need JsonConverter
            $result += "public enum $className : $backingType`n{"
        } else {
            # String enums need JsonEnumMemberConverter
            $result += "[JsonConverter(typeof(JsonEnumMemberConverter<$className>))]`n"
            $result += "public enum $className`n{"
        }

        # Generate enum values from the enum array
        $enumValues = @()
        foreach ($enumValue in $Definition.enum) {
            # Convert enum value to valid C# identifier
            $enumName = Convert-PropertyName $enumValue.ToString()

            # Add JsonEnumValue attribute for string enums
            if ($enumType -eq "string") {
                $enumValues += "[JsonEnumValue(`"$enumValue`")]`n    $enumName"
            } else {
                # For integer enums, add the explicit value
                $enumValues += "$enumName = $enumValue"
            }
        }

        # Join enum values with commas
        $result += "`n    " + ($enumValues -join ",`n`n    ") + "`n"
        $result += "}"

        return $result
    }

    # Handle oneOf/anyOf at root level (union types or enums)
    if ($Definition.oneOf -or $Definition.anyOf) {
        $items = @(if ($Definition.oneOf) { $Definition.oneOf } else { $Definition.anyOf })

        # Check if this is an enum-like pattern (all items have same type with const or title)
        $allHaveConstOrTitle = $true
        $allSameType = $true
        $firstType = $null

        foreach ($item in $items) {
            $itemType = $item.type
            if ($itemType -is [array]) {
                $itemType = ($itemType | Where-Object { $_ -ne "null" })[0]
            }

            # Track if all items have the same type
            if ($null -eq $firstType) {
                $firstType = $itemType
            } elseif ($firstType -ne $itemType) {
                $allSameType = $false
                break
            }

            # An item can be treated as enum if it has const OR if it has title (as a fallback const)
            if (-not $item.const -and -not $item.title) {
                $allHaveConstOrTitle = $false
                break
            }
        }

        # If all items have same type and const/title, it's an enum
        # Support both string and integer enums
        if ($allHaveConstOrTitle -and $allSameType -and ($firstType -eq "string" -or $firstType -eq "integer")) {
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

            # For integer enums, specify the backing type (no JsonConverter needed - use default serialization)
            # For string enums, use JsonEnumMemberConverter with JsonEnumValue attributes
            if ($firstType -eq "integer") {
                # Check format for backing type
                $backingType = "int"
                if ($items[0].format) {
                    switch ($items[0].format) {
                        "int64" { $backingType = "long" }
                        "uint16" { $backingType = "ushort" }
                        "uint32" { $backingType = "uint" }
                        "uint64" { $backingType = "ulong" }
                        "int16" { $backingType = "short" }
                        "int32" { $backingType = "int" }
                        default { $backingType = "int" }
                    }
                }
                # Integer enums don't need JsonConverter - they serialize naturally as numbers
                $result += "public enum $className : $backingType`n{"
            } else {
                # String enums need JsonEnumMemberConverter to handle JsonEnumValue attributes
                $result += "[JsonConverter(typeof(JsonEnumMemberConverter<$className>))]`n"
                $result += "public enum $className`n{"
            }

            # Generate enum values
            $enumValues = @()
            foreach ($item in $items) {
                # Use const if available, otherwise use title
                $constValue = if ($item.const -ne $null) { $item.const } else { $item.title }

                # Generate enum name
                if ($item.title) {
                    # Use title and convert to PascalCase
                    $enumName = Convert-PropertyName $item.title
                } else {
                    # Convert const value to string and use it
                    $enumName = Convert-PropertyName $constValue.ToString()
                }

                # For multi-word titles with spaces, convert to PascalCase properly
                if ($enumName -match '\s') {
                    $words = $enumName -split '\s+'
                    $enumName = ($words | ForEach-Object { 
                        if ($_.Length -gt 0) {
                            $_.Substring(0, 1).ToUpper() + $_.Substring(1).ToLower()
                        }
                    }) -join ''
                }

                # Remove any remaining invalid characters
                $enumName = $enumName -replace '[^a-zA-Z0-9_]', ''

                # Ensure it starts with a letter or underscore
                if ($enumName -match '^\d') {
                    $enumName = "_" + $enumName
                }

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

                # For integer enums, always include the value (but skip if const is null)
                # For string enums, add JsonEnumValue attribute if the enum name differs from the const value
                if ($firstType -eq "integer") {
                    if ($item.const -ne $null) {
                        $enumEntry += "    $enumName = $constValue"
                    } else {
                        # This item has no const value, it's a catch-all - use a safe default value
                        # Find a value that doesn't conflict
                        $enumEntry += "    $enumName = 0"
                    }
                } else {
                    if ($enumName -cne $constValue) {
                        $enumEntry += "    [JsonEnumValue(`"$constValue`")]`n"
                    }
                    $enumEntry += "    $enumName"
                }

                $enumValues += $enumEntry
            }

            $result += "`n"
            $result += ($enumValues -join ",`n`n") + "`n"
            $result += "}"

            return $result
        } 

        # Check if this is a union type (different types, no properties)
        $hasProperties = $false
        $unionTypes = @()
        $hasNullType = $false

        foreach ($item in $items) {
            if ($item.properties) {
                $hasProperties = $true
                break
            }

            $itemType = $item.type
            if ($itemType -is [array]) {
                # Check if array contains null
                if ("null" -in $itemType) {
                    $hasNullType = $true
                }
                # Skip null types for union type list
                $itemType = $itemType | Where-Object { $_ -ne "null" }
                if ($itemType.Count -gt 0) {
                    $itemType = $itemType[0]
                } else {
                    continue
                }
            } elseif ($itemType -eq "null") {
                $hasNullType = $true
                continue
            }

            if ($itemType) {
                # Map JSON type to C# type with format consideration
                $csType = if ($itemType -eq "integer" -and $item.format) {
                    switch ($item.format) {
                        "int64" { "long" }
                        "uint16" { "ushort" }
                        "uint32" { "uint" }
                        "uint64" { "ulong" }
                        "int16" { "short" }
                        "int32" { "int" }
                        default { "int" }
                    }
                } else {
                    Get-TypeName $itemType
                }
                $unionTypes += $csType
            }
        }

        # If we have multiple types and no properties, it's a union type
        if ($unionTypes.Count -gt 1 -and -not $hasProperties) {
            return New-UnionTypeStruct $Name $Definition $unionTypes $hasNullType
        }

        # Otherwise it's a discriminated union - try to extract properties from union items
        if ($hasProperties) {
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
            # Don't add ? if there's a default value (means it's initialized and not null)
            if (-not $propIsRequired -and -not $csType.EndsWith('?') -and $null -eq $prop.default) {
                # Only add ? for value types, not reference types
                if ($csType -in @('int', 'bool', 'double', 'uint', 'ushort', 'ulong', 'short', 'long', 'byte', 'sbyte', 'float', 'decimal')) {
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
                $propLine += "    [JsonProperty(`"$propName`")]`n"
            }

            # Build property declaration - use -f formatting instead of interpolation
            $propDeclaration = "    public {0} {1} {{ get; set; }}" -f $csType, $csPropName

            if ($propIsRequired -and -not $csType.EndsWith('?')) {
                # Only add = null! for reference types, not value types
                # Value types: primitive types, enums, type aliases, and union types (all structs)
                # Reference types: string, object, List, Dictionary, and custom classes
                $valueTypes = @('int', 'bool', 'double', 'uint', 'ushort', 'ulong', 'short', 'long', 'byte', 'sbyte', 'float', 'decimal', 'char')
                $typeAliases = @('PermissionOptionId', 'ProtocolVersion', 'SessionConfigGroupId', 'SessionConfigId', 'SessionConfigValueId', 'SessionId', 'SessionModeId', 'ToolCallId', 'RequestId', 'SessionConfigSelectOptions')
                $enumTypes = @('PermissionOptionKind', 'PlanEntryPriority', 'PlanEntryStatus', 'StopReason', 'ToolCallStatus', 'ToolKind', 'SessionConfigOptionCategory', 'ErrorCode')

                $isValueType = ($csType -in $valueTypes) -or ($csType -in $typeAliases) -or ($csType -in $enumTypes)
                $isReferenceType = -not $isValueType

                if ($isReferenceType) {
                    $propDeclaration += " = null!;"
                }
            }

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
        $result += ($properties -join "`n`n") + "`n"
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

# Only run main execution if this script is invoked directly, not when dot-sourced
if ($MyInvocation.InvocationName -ne ".") {

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

# Generate using statements
$usings = @(
    "using System;"
    "using System.Collections.Generic;"
    "using Newtonsoft.Json;"
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
    $typeAliases = @()
    $enumDefinitions = @()
    $recordClasses = @()

    foreach ($defName in ($defs.PSObject.Properties.Name | Sort-Object)) {
        $def = $defs.$defName
        $classCode = New-ModelClass $defName $def $defs

        # Check if this is a type alias struct (has IEquatable)
        if ($classCode -match 'IEquatable<\w+>') {
            $typeAliases += $classCode
        }
        # Check if this is an enum definition
        # Enums can have XML docs before the enum statement
        elseif ($classCode -match 'public\s+enum\s+\w+') {
            $enumDefinitions += $classCode
        } else {
            $recordClasses += $classCode
        }
    }

    # Add type aliases first
    if ($typeAliases.Count -gt 0) {
        $output += "    // Type aliases"
        $output += ""
        foreach ($alias in $typeAliases) {
            # Indent type alias definitions
            $indentedAlias = $alias -split "`n" | ForEach-Object { if ($_.Length -gt 0) { "    $_" } else { "" } }
            $output += $indentedAlias -join "`n"
            $output += ""
        }
    }

    # Add enums next
    if ($enumDefinitions.Count -gt 0) {
        $output += "    // Enums for string-based enum-like types"
        $output += ""
        foreach ($enum in $enumDefinitions) {
            # Indent enum definitions
            $indentedEnum = $enum -split "`n" | ForEach-Object { if ($_.Length -gt 0) { "    $_" } else { "" } }
            $output += $indentedEnum -join "`n"
            $output += ""
        }
    }

    # Then add class definitions
    $output += "    // Generated model classes from ACP schema"
    $output += ""
    foreach ($recordClass in $recordClasses) {
        # Indent class definitions
        $indentedClass = $recordClass -split "`n" | ForEach-Object { if ($_.Length -gt 0) { "    $_" } else { "" } }
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

Write-Host "  [OK] Generated C# models at $OutputPath" -ForegroundColor Gray

} # End of "if ($MyInvocation.InvocationName -ne ".")" block
