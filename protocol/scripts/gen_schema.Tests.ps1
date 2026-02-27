Describe "Convert-PropertyName" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "converts snake_case to PascalCase" {
        $result = Convert-PropertyName "load_session"
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

    It "converts object to object" {
        $result = Get-TypeName "object"
        ($result -ceq "object") | Should Be $true
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

    It "does not add question mark to string when nullable" {
        $property = @{ type = @("string", "null") }
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
            type = @("string", "null")
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

Describe "Get-PropertyType - Nullable Value Types with Format Hints" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    Context "uint format hints" {
        It "converts ['integer', 'null'] with format uint32 to uint?" {
            $property = @{
                type = @("integer", "null")
                format = "uint32"
            }
            $result = Get-PropertyType $property $null
            $result | Should Be "uint?"
        }

        It "converts ['integer', 'null'] with format uint64 to ulong?" {
            $property = @{
                type = @("integer", "null")
                format = "uint64"
            }
            $result = Get-PropertyType $property $null
            $result | Should Be "ulong?"
        }

        It "converts ['integer', 'null'] with format uint16 to ushort?" {
            $property = @{
                type = @("integer", "null")
                format = "uint16"
            }
            $result = Get-PropertyType $property $null
            $result | Should Be "ushort?"
        }
    }

    Context "int format hints" {
        It "converts ['integer', 'null'] with format int32 to int?" {
            $property = @{
                type = @("integer", "null")
                format = "int32"
            }
            $result = Get-PropertyType $property $null
            $result | Should Be "int?"
        }

        It "converts ['integer', 'null'] with format int64 to long?" {
            $property = @{
                type = @("integer", "null")
                format = "int64"
            }
            $result = Get-PropertyType $property $null
            $result | Should Be "long?"
        }

        It "converts ['integer', 'null'] with format int16 to short?" {
            $property = @{
                type = @("integer", "null")
                format = "int16"
            }
            $result = Get-PropertyType $property $null
            $result | Should Be "short?"
        }
    }

    Context "other nullable value types" {
        It "converts ['integer', 'null'] without format to int?" {
            $property = @{
                type = @("integer", "null")
            }
            $result = Get-PropertyType $property $null
            $result | Should Be "int?"
        }

        It "converts ['boolean', 'null'] to bool?" {
            $property = @{
                type = @("boolean", "null")
            }
            $result = Get-PropertyType $property $null
            $result | Should Be "bool?"
        }

        It "converts ['number', 'null'] to double?" {
            $property = @{
                type = @("number", "null")
            }
            $result = Get-PropertyType $property $null
            $result | Should Be "double?"
        }
    }
}

Describe "Get-PropertyType - Nullable Reference Types" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "converts ['string', 'null'] to string (NOT string?)" {
        $property = @{
            type = @("string", "null")
        }
        $result = Get-PropertyType $property $null
        $result | Should Be "string"
        $result | Should Not Be "string?"
    }

    It "converts ['object', 'null'] to object" {
        $property = @{
            type = @("object", "null")
        }
        $result = Get-PropertyType $property $null
        $result | Should Be "object"
    }

    It "converts ['array', 'null'] to List (NOT List?)" {
        $property = @{
            type = @("array", "null")
        }
        $result = Get-PropertyType $property $null
        $result | Should Be "List<object>"
    }
}

Describe "Get-PropertyType - anyOf with Reference + null Pattern" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "converts anyOf with ref to Annotations and type null to Annotations" {
        $property = @{
            anyOf = @(
                @{ '$ref' = '#/$defs/Annotations' },
                @{ type = "null" }
            )
        }
        $result = Get-PropertyType $property $null
        $result | Should Be "Annotations"
    }

    It "converts anyOf with type null and ref to Implementation (reverse order)" {
        $property = @{
            anyOf = @(
                @{ type = "null" },
                @{ '$ref' = '#/$defs/Implementation' }
            )
        }
        $result = Get-PropertyType $property $null
        $result | Should Be "Implementation"
    }

    It "converts anyOf with ref to SessionModeState and type null" {
        $property = @{
            anyOf = @(
                @{ '$ref' = '#/$defs/SessionModeState' },
                @{ type = "null" }
            )
        }
        $result = Get-PropertyType $property $null
        $result | Should Be "SessionModeState"
    }

    It "converts anyOf with ref to TerminalExitStatus and type null" {
        $property = @{
            anyOf = @(
                @{ '$ref' = '#/$defs/TerminalExitStatus' },
                @{ type = "null" }
            )
        }
        $result = Get-PropertyType $property $null
        $result | Should Be "TerminalExitStatus"
    }
}

Describe "Get-PropertyType - Complex anyOf Patterns" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "converts complex anyOf (3+ items) to object" {
        $property = @{
            anyOf = @(
                @{ '$ref' = '#/$defs/TypeA' },
                @{ '$ref' = '#/$defs/TypeB' },
                @{ '$ref' = '#/$defs/TypeC' }
            )
        }
        $result = Get-PropertyType $property $null
        $result | Should Be "object"
    }

    It "converts anyOf with allOf and refs to object" {
        $property = @{
            anyOf = @(
                @{ allOf = @(@{ '$ref' = '#/$defs/ResponseA' }) },
                @{ allOf = @(@{ '$ref' = '#/$defs/ResponseB' }) }
            )
        }
        $result = Get-PropertyType $property $null
        $result | Should Be "object"
    }
}

Describe "anyOf Enum Detection" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "generates string enum for anyOf with all string const values" {
        $definition = @{
            description = "Test enum"
            anyOf = @(
                @{ type = "string"; const = "value1"; description = "First value" },
                @{ type = "string"; const = "value2"; description = "Second value" }
            )
        }

        $result = New-ModelClass "TestEnum" $definition @{}

        ($result -match "public enum TestEnum") | Should Be $true
        ($result -match 'JsonEnumValue\("value1"\)') | Should Be $true
        ($result -match 'JsonEnumValue\("value2"\)') | Should Be $true
        ($result -match "Value1") | Should Be $true
        ($result -match "Value2") | Should Be $true
    }

    It "generates string enum for anyOf with title fallback" {
        $definition = @{
            description = "Test enum with titles"
            anyOf = @(
                @{ type = "string"; title = "Mode"; const = "mode" },
                @{ type = "string"; title = "Other" }
            )
        }

        $result = New-ModelClass "TestCategory" $definition @{}

        ($result -match "public enum TestCategory") | Should Be $true
        ($result -match 'JsonEnumValue\("mode"\)') | Should Be $true
        ($result -match "Mode") | Should Be $true
        ($result -match "Other") | Should Be $true
    }

    It "generates integer enum for anyOf with all integer const values" {
        $definition = @{
            description = "Error codes"
            anyOf = @(
                @{ type = "integer"; format = "int32"; const = -32700; title = "Parse error"; description = "Parse error" },
                @{ type = "integer"; format = "int32"; const = -32600; title = "Invalid request"; description = "Invalid request" },
                @{ type = "integer"; format = "int32"; title = "Other" }
            )
        }

        $result = New-ModelClass "ErrorCode" $definition @{}

        ($result -match "public enum ErrorCode") | Should Be $true
        ($result -match ": int") | Should Be $true
        ($result -match "ParseError = -32700") | Should Be $true
        ($result -match "InvalidRequest = -32600") | Should Be $true
        ($result -match "Other = 0") | Should Be $true
    }

    It "generates long-backed enum for int64 format" {
        $definition = @{
            anyOf = @(
                @{ type = "integer"; format = "int64"; const = 9223372036854775807; title = "MaxLong" }
            )
        }

        $result = New-ModelClass "LongEnum" $definition @{}

        ($result -match "public enum LongEnum") | Should Be $true
        ($result -match ": long") | Should Be $true
    }
}

Describe "anyOf Union Type Detection" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "generates union type for anyOf with mixed types" {
        $definition = @{
            description = "Request ID"
            anyOf = @(
                @{ type = "null"; title = "Null" },
                @{ type = "integer"; format = "int64"; title = "Number" },
                @{ type = "string"; title = "Str" }
            )
        }

        $result = New-ModelClass "RequestId" $definition @{}

        ($result -match "public readonly struct RequestId") | Should Be $true
        ($result -match "IEquatable<RequestId>") | Should Be $true
        ($result -match "UnionTypeConverter<RequestId>") | Should Be $true
        ($result -match "public RequestId\(long value\)") | Should Be $true
        ($result -match "public RequestId\(string value\)") | Should Be $true
        ($result -match "public bool IsNull") | Should Be $true
        ($result -match "public static RequestId Null") | Should Be $true
        ($result -match "public bool TryGetLong\(out long value\)") | Should Be $true
        ($result -match "public bool TryGetString\(out string value\)") | Should Be $true
    }

    It "generates union without null flag when no null type" {
        $definition = @{
            anyOf = @(
                @{ type = "integer"; format = "int32" },
                @{ type = "string" }
            )
        }

        $result = New-ModelClass "SimpleUnion" $definition @{}

        ($result -match "public readonly struct SimpleUnion") | Should Be $true
        ($result -notmatch "private readonly bool _isNull") | Should Be $true
        ($result -notmatch "public bool IsNull") | Should Be $true
    }
}

Describe "anyOf vs Discriminated Union" {
    BeforeAll {
        . "$PSScriptRoot\gen_schema.ps1"
    }

    It "generates discriminated union class when anyOf has properties" {
        $definition = @{
            anyOf = @(
                @{ 
                    type = "object"
                    properties = @{
                        type = @{ type = "string"; const = "text" }
                        text = @{ type = "string" }
                    }
                    required = @("type", "text")
                },
                @{ 
                    type = "object"
                    properties = @{
                        type = @{ type = "string"; const = "image" }
                        data = @{ type = "string" }
                    }
                    required = @("type", "data")
                }
            )
        }

        $result = New-ModelClass "Content" $definition @{}

        # Should generate class, not enum or union struct
        ($result -match "public class Content") | Should Be $true
        ($result -notmatch "public enum") | Should Be $true
        ($result -notmatch "UnionTypeConverter") | Should Be $true
    }
}

Describe "simple enum" {
    It "generates string enum from simple enum definition" {
        $definition = @{
            description = "The sender or recipient of messages and data in a conversation."
            type = "string"
            enum = @("assistant", "user")
        }

        $result = New-ModelClass "Role" $definition @{}

        ($result -match "public enum Role") | Should Be $true
        ($result -match "JsonEnumMemberConverter<Role>") | Should Be $true
        ($result -match 'JsonEnumValue\("assistant"\)') | Should Be $true
        ($result -match 'JsonEnumValue\("user"\)') | Should Be $true
        ($result -match "Assistant") | Should Be $true
        ($result -match "User") | Should Be $true
        ($result -match "The sender or recipient") | Should Be $true
    }

    It "generates integer enum from simple enum definition" {
        $definition = @{
            description = "HTTP status codes"
            type = "integer"
            format = "uint32"
            enum = @(200, 404, 500)
        }

        $result = New-ModelClass "HttpStatus" $definition @{}

        ($result -match "public enum HttpStatus") | Should Be $true
        ($result -match ": uint") | Should Be $true
        ($result -match "200") | Should Be $true
        ($result -match "404") | Should Be $true
        ($result -match "500") | Should Be $true
    }
}
