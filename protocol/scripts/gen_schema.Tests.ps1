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
