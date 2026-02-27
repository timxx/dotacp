Describe "Convert-PropertyName" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "converts snake_case to PascalCase" {
        $result = Convert-PropertyName "load_session"
        $result | Should Be "LoadSession"
        ($result -ceq "LoadSession") | Should Be $true
    }

    It "converts multiple underscores correctly" {
        $result = Convert-PropertyName "read_text_file"
        ($result -ceq "ReadTextFile") | Should Be $true
    }

    It "handles special case _meta correctly" {
        $result = Convert-PropertyName "_meta"
        ($result -ceq "Meta") | Should Be $true
    }

    It "handles single word unchanged" {
        $result = Convert-PropertyName "name"
        ($result -ceq "Name") | Should Be $true
    }

    It "preserves existing capitalization within words" {
        $result = Convert-PropertyName "data_id"
        ($result -ceq "DataId") | Should Be $true
    }
}

Describe "Convert-NameToClass" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "capitalizes first letter for class names" {
        $result = Convert-NameToClass "content"
        ($result -ceq "Content") | Should Be $true
    }

    It "handles already capitalized names" {
        $result = Convert-NameToClass "Content"
        ($result -ceq "Content") | Should Be $true
    }

    It "capitalizes lowercase single letter" {
        $result = Convert-NameToClass "a"
        ($result -ceq "A") | Should Be $true
    }

    It "handles empty string" {
        $result = Convert-NameToClass ""
        ($result -ceq "") | Should Be $true
    }
}

Describe "Get-TypeName" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "converts string type" {
        $result = Get-TypeName "string"
        ($result -ceq "string") | Should Be $true
    }

    It "converts number to double" {
        $result = Get-TypeName "number"
        ($result -ceq "double") | Should Be $true
    }

    It "converts integer to int" {
        $result = Get-TypeName "integer"
        ($result -ceq "int") | Should Be $true
    }

    It "converts boolean to bool" {
        $result = Get-TypeName "boolean"
        ($result -ceq "bool") | Should Be $true
    }

    It "converts object to Dictionary" {
        $result = Get-TypeName "object"
        ($result -ceq "Dictionary<string, object>") | Should Be $true
    }

    It "converts array to List" {
        $result = Get-TypeName "array"
        ($result -ceq "List<object>") | Should Be $true
    }

    It "handles unknown types as object" {
        $result = Get-TypeName "unknown"
        ($result -ceq "object") | Should Be $true
    }
}

Describe "Convert-DefaultValue" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "converts bool true to true" {
        $result = Convert-DefaultValue $true "bool?"
        ($result -ceq "true") | Should Be $true
    }

    It "converts bool false to false" {
        $result = Convert-DefaultValue $false "bool?"
        ($result -ceq "false") | Should Be $true
    }

    It "converts int value" {
        $result = Convert-DefaultValue 42 "int?"
        ($result -eq 42) | Should Be $true
    }

    It "converts double value" {
        $result = Convert-DefaultValue 3.14 "double?"
        ($result -eq 3.14) | Should Be $true
    }

    It "returns null for null value" {
        $result = Convert-DefaultValue $null "bool?"
        ($result -eq $null) | Should Be $true
    }

    It "returns null for List type with null value" {
        $result = Convert-DefaultValue $null "List<string>"
        ($result -eq $null) | Should Be $true
    }

    It "returns null for Dictionary type with null value" {
        $result = Convert-DefaultValue $null "Dictionary<string, object>"
        ($result -eq $null) | Should Be $true
    }

    It "generates new List syntax for non-null value" {
        $result = Convert-DefaultValue "dummy" "List<string>"
        ($result -ceq "new List<string>()") | Should Be $true
    }
}

Describe "Get-PropertyType" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "extracts class name from ref path" {
        $property = @{ '$ref' = '#/$defs/MyClass' }
        $result = Get-PropertyType $property @{}
        ($result -ceq "MyClass") | Should Be $true
    }

    It "wraps array item type in List" {
        $property = @{ type = "array"; items = @{ type = "string" } }
        $result = Get-PropertyType $property @{}
        ($result -ceq "List<string>") | Should Be $true
    }

    It "defaults to List object for arrays without items" {
        $property = @{ type = "array" }
        $result = Get-PropertyType $property @{}
        ($result -ceq "List<object>") | Should Be $true
    }

    It "handles type arrays with null correctly" {
        $property = @{ type = @("string", $null) }
        $result = Get-PropertyType $property @{}
        ($result -ceq "string") | Should Be $true
    }

    It "does not add question mark to string when nullable" {
        $property = @{ type = @("string", $null) }
        $result = Get-PropertyType $property @{}
        ($result -ceq "string") | Should Be $true
    }

    It "returns string for enum type" {
        $property = @{ type = "string"; enum = @("value1", "value2") }
        $result = Get-PropertyType $property @{}
        ($result -ceq "string") | Should Be $true
    }

    It "converts string type" {
        $property = @{ type = "string" }
        $result = Get-PropertyType $property @{}
        ($result -ceq "string") | Should Be $true
    }

    It "converts integer type" {
        $property = @{ type = "integer" }
        $result = Get-PropertyType $property @{}
        ($result -ceq "int") | Should Be $true
    }

    It "converts boolean type" {
        $property = @{ type = "boolean" }
        $result = Get-PropertyType $property @{}
        ($result -ceq "bool") | Should Be $true
    }

    It "returns object for anyOf union" {
        $property = @{ anyOf = @(@{ type = "string" }, @{ type = "number" }) }
        $result = Get-PropertyType $property @{}
        ($result -ceq "object") | Should Be $true
    }

    It "returns object for oneOf without const" {
        $property = @{ oneOf = @(@{ type = "string" }, @{ type = "number" }) }
        $result = Get-PropertyType $property @{}
        ($result -ceq "object") | Should Be $true
    }
}

Describe "Case-sensitive property name detection" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "detects case difference between JSON and C# names" {
        $jsonName = "sessionUpdate"
        $csharpName = "SessionUpdate"

        ($jsonName -cne $csharpName) | Should Be $true
    }

    It "handles meta property correctly" {
        $jsonName = "_meta"
        $csharpName = Convert-PropertyName $jsonName

        ($csharpName -ceq "Meta") | Should Be $true
        ($jsonName -cne $csharpName) | Should Be $true
    }

    It "case-sensitive comparison catches case differences" {
        $name1 = "test"
        $name2 = "TEST"

        ($name1 -eq $name2) | Should Be $true
        ($name1 -ceq $name2) | Should Be $false
    }
}

Describe "Enum Schema Value Mapping" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "uses generic JsonEnumMemberConverter" {
        $definition = @{
            oneOf = @(
                @{ const = "value_one"; type = "string"; description = "Value one" }
            )
            description = "Test enum"
        }

        $result = New-ModelClass "MyEnum" $definition @{}

        ($result -like "*JsonEnumMemberConverter<MyEnum>*") | Should Be $true
    }

    It "adds JsonEnumValue attribute when name differs" {
        $definition = @{
            oneOf = @(
                @{ const = "allow_once"; type = "string"; description = "Allow once" }
            )
            description = "Test enum"
        }

        $result = New-ModelClass "TestEnum" $definition @{}

        # Use match to avoid bracket parsing issues
        ($result -match 'JsonEnumValue.*allow_once') | Should Be $true
    }
}

Describe "Test-IsSimpleTypeAlias" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "returns true for simple string type" {
        $definition = @{ 
            type = "string"
            description = "A simple string type alias"
        }
        $result = Test-IsSimpleTypeAlias $definition
        $result | Should Be $true
    }

    It "returns true for integer type with format" {
        $definition = @{ 
            type = "integer"
            format = "uint16"
            description = "Protocol version"
        }
        $result = Test-IsSimpleTypeAlias $definition
        $result | Should Be $true
    }

    It "returns false for type with properties" {
        $definition = @{ 
            type = "object"
            properties = @{ name = @{ type = "string" } }
        }
        $result = Test-IsSimpleTypeAlias $definition
        $result | Should Be $false
    }

    It "returns false for type with oneOf" {
        $definition = @{ 
            oneOf = @(
                @{ type = "string"; const = "value1" },
                @{ type = "string"; const = "value2" }
            )
        }
        $result = Test-IsSimpleTypeAlias $definition
        $result | Should Be $false
    }

    It "returns false for type with enum" {
        $definition = @{ 
            type = "string"
            enum = @("value1", "value2")
        }
        $result = Test-IsSimpleTypeAlias $definition
        $result | Should Be $false
    }

    It "returns false for array type" {
        $definition = @{ 
            type = "array"
            items = @{ type = "string" }
        }
        $result = Test-IsSimpleTypeAlias $definition
        $result | Should Be $false
    }

    It "returns false for type array (nullable)" {
        $definition = @{ 
            type = @("string", $null)
        }
        $result = Test-IsSimpleTypeAlias $definition
        $result | Should Be $false
    }

    It "returns false when type is missing" {
        $definition = @{ 
            description = "No type specified"
        }
        $result = Test-IsSimpleTypeAlias $definition
        $result | Should Be $false
    }
}

Describe "Get-TypeAliasTarget" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "returns string for string type" {
        $definition = @{ type = "string" }
        $result = Get-TypeAliasTarget $definition
        ($result -ceq "string") | Should Be $true
    }

    It "returns int for integer type without format" {
        $definition = @{ type = "integer" }
        $result = Get-TypeAliasTarget $definition
        ($result -ceq "int") | Should Be $true
    }

    It "returns ushort for uint16 format" {
        $definition = @{ type = "integer"; format = "uint16" }
        $result = Get-TypeAliasTarget $definition
        ($result -ceq "ushort") | Should Be $true
    }

    It "returns uint for uint32 format" {
        $definition = @{ type = "integer"; format = "uint32" }
        $result = Get-TypeAliasTarget $definition
        ($result -ceq "uint") | Should Be $true
    }

    It "returns ulong for uint64 format" {
        $definition = @{ type = "integer"; format = "uint64" }
        $result = Get-TypeAliasTarget $definition
        ($result -ceq "ulong") | Should Be $true
    }

    It "returns long for int64 format" {
        $definition = @{ type = "integer"; format = "int64" }
        $result = Get-TypeAliasTarget $definition
        ($result -ceq "long") | Should Be $true
    }

    It "returns bool for boolean type" {
        $definition = @{ type = "boolean" }
        $result = Get-TypeAliasTarget $definition
        ($result -ceq "bool") | Should Be $true
    }
}

Describe "New-TypeAliasStruct" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "generates struct for string type alias" {
        $definition = @{ 
            type = "string"
            description = "Test string alias"
        }
        $result = New-TypeAliasStruct "TestId" $definition "string"
        
        $result | Should Match "public readonly struct TestId"
        $result | Should Match "IEquatable<TestId>"
        $result | Should Match "private readonly string _value"
        $result | Should Match "Test string alias"
    }

    It "generates struct for ushort type alias" {
        $definition = @{ 
            type = "integer"
            format = "uint16"
            description = "Test version number"
        }
        $result = New-TypeAliasStruct "TestVersion" $definition "ushort"
        
        $result | Should Match "public readonly struct TestVersion"
        $result | Should Match "IEquatable<TestVersion>"
        $result | Should Match "private readonly ushort _value"
        $result | Should Match "Test version number"
    }

    It "includes implicit conversion operators" {
        $definition = @{ type = "string" }
        $result = New-TypeAliasStruct "TestId" $definition "string"
        
        $result | Should Match "public static implicit operator TestId\(string value\)"
        $result | Should Match "public static implicit operator string\(TestId alias\)"
    }

    It "includes Equals, GetHashCode, and ToString methods" {
        $definition = @{ type = "string" }
        $result = New-TypeAliasStruct "TestId" $definition "string"
        
        $result | Should Match "public bool Equals\(TestId other\)"
        $result | Should Match "public override bool Equals\(object obj\)"
        $result | Should Match "public override int GetHashCode\(\)"
        $result | Should Match "public override string ToString\(\)"
    }

    It "handles null for string types in GetHashCode" {
        $definition = @{ type = "string" }
        $result = New-TypeAliasStruct "TestId" $definition "string"
        
        $result | Should Match "_value\?\.GetHashCode\(\) \?\? 0"
    }

    It "does not handle null for value types in GetHashCode" {
        $definition = @{ type = "integer"; format = "uint16" }
        $result = New-TypeAliasStruct "TestVersion" $definition "ushort"
        
        $result | Should Not Match "_value\?\.GetHashCode"
        $result | Should Match "_value\.GetHashCode\(\)"
    }

    It "includes JsonConverter attribute" {
        $definition = @{ type = "string" }
        $result = New-TypeAliasStruct "TestId" $definition "string"
        
        $result | Should Match "\[JsonConverter\(typeof\(TypeAliasConverter<TestId, string>\)\)\]"
    }
}
