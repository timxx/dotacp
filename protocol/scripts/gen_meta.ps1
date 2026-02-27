#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generate Meta.cs from meta.json

.DESCRIPTION
    Generates C# constants and method mappings from meta.json
    This includes:
    - AgentMethods: methods that agents can handle
    - ClientMethods: methods that clients can handle
    - PROTOCOL_VERSION: the ACP protocol version
#>

$ErrorActionPreference = "Stop"

$ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProtocolDir = Split-Path -Parent $ScriptRoot
$RepoRoot = Split-Path -Parent $ProtocolDir
$SchemaDir = Join-Path $ProtocolDir "schema"
$MetaJsonPath = Join-Path $SchemaDir "meta.json"
$VersionFile = Join-Path $SchemaDir "VERSION"
$OutputPath = Join-Path $ProtocolDir "Meta.cs"

if (-not (Test-Path $MetaJsonPath)) {
    Write-Error "schema/meta.json not found" -ErrorAction Stop
}

Write-Host "  Parsing meta.json..." -ForegroundColor Gray
$meta = Get-Content $MetaJsonPath -Raw | ConvertFrom-Json

# Generate header
$headerLines = @()
$headerLines += "// Generated from schema/meta.json. Do not edit by hand."

if (Test-Path $VersionFile) {
    $ref = (Get-Content $VersionFile).Trim()
    if ($ref) {
        $headerLines += "// Schema ref: $ref"
    }
}

$header = $headerLines -join "`n"

# Helper function to convert snake_case to PascalCase
function ConvertTo-PascalCase {
    param([string]$name)
    
    $parts = $name -split '_'
    $pascalCase = ($parts | ForEach-Object { 
        if ($_.Length -gt 0) {
            $_.Substring(0, 1).ToUpper() + $_.Substring(1).ToLower()
        }
    }) -join ''
    
    return $pascalCase
}

# Build output
$output = @()
$output += $header
$output += ""
$output += "namespace dotacp.protocol"
$output += "{"
$output += "    /// <summary>"
$output += "    /// Protocol metadata"
$output += "    /// </summary>"
$output += "    public static class ProtocolMeta"
$output += "    {"

# Add protocol version
$version = $(if ($meta.version) { $meta.version } else { 1 })
$output += "        /// <summary>"
$output += "        /// ACP Protocol Version"
$output += "        /// </summary>"
$output += "        public const ushort Version = $version;"
$output += "    }"
$output += ""

# Add agent methods as top-level static class
$output += "    /// <summary>"
$output += "    /// Methods that agents handle"
$output += "    /// </summary>"
$output += "    public static class AgentMethods"
$output += "    {"

if ($meta.agentMethods) {
    foreach ($methodName in ($meta.agentMethods.PSObject.Properties.Name | Sort-Object)) {
        $methodPath = $meta.agentMethods.$methodName
        $constName = ConvertTo-PascalCase $methodName
        $output += "        public const string $constName = `"$methodPath`";"
    }
}

$output += "    }"
$output += ""

# Add client methods as top-level static class
$output += "    /// <summary>"
$output += "    /// Methods that clients handle"
$output += "    /// </summary>"
$output += "    public static class ClientMethods"
$output += "    {"

if ($meta.clientMethods) {
    foreach ($methodName in ($meta.clientMethods.PSObject.Properties.Name | Sort-Object)) {
        $methodPath = $meta.clientMethods.$methodName
        $constName = ConvertTo-PascalCase $methodName
        $output += "        public const string $constName = `"$methodPath`";"
    }
}

$output += "    }"
$output += "}"

# Write output with proper formatting
$finalOutput = $output -join "`n"
if (-not $finalOutput.EndsWith("`n")) {
    $finalOutput += "`n"
}

Set-Content -Path $OutputPath -Value $finalOutput -Encoding UTF8
Write-Host "  [OK] Generated meta definitions at $OutputPath" -ForegroundColor Gray
