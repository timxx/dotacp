#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Generate Meta.cs from meta.json

.DESCRIPTION
    Wrapper script that calls the C# implementation of meta code generation.
    If the generator executable is not found, it will automatically build it.
#>

param(
    [string]$SchemaDir = "",
    [string]$OutputDir = ""
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$protocolRoot = Split-Path -Parent $scriptRoot
$repoRoot = Split-Path -Parent $protocolRoot
$generatorDir = Join-Path $repoRoot "generator"

# Find the generator executable (check Release first, then Debug, then root bin)
$generatorExe = @(
    (Join-Path (Join-Path $generatorDir "bin") "Release" "dotacp.generator.exe"),
    (Join-Path (Join-Path $generatorDir "bin") "Debug" "dotacp.generator.exe"),
    (Join-Path $generatorDir "bin" "dotacp.generator.exe")
) | Where-Object { Test-Path $_ } | Select-Object -First 1

# Use defaults if not provided
if ([string]::IsNullOrEmpty($SchemaDir)) {
    $SchemaDir = Join-Path $protocolRoot "schema"
}
if ([string]::IsNullOrEmpty($OutputDir)) {
    $OutputDir = $protocolRoot
}

# Check if executable exists, if not compile it
if (-not $generatorExe -or -not (Test-Path $generatorExe)) {
    Write-Host "  Generator executable not found, building project..." -ForegroundColor Yellow
    dotnet build "$generatorDir\generator.csproj" -c Release
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to build generator"
        exit 1
    }
    # Find the newly built executable
    $generatorExe = @(
        (Join-Path (Join-Path $generatorDir "bin") "Release" "dotacp.generator.exe"),
        (Join-Path (Join-Path $generatorDir "bin") "Debug" "dotacp.generator.exe"),
        (Join-Path $generatorDir "bin" "dotacp.generator.exe")
    ) | Where-Object { Test-Path $_ } | Select-Object -First 1
    
    if (-not $generatorExe) {
        Write-Error "Generator executable not found after build"
        exit 1
    }
}

# Call the C# generator
& $generatorExe meta --schema-dir "$SchemaDir" --output-dir "$OutputDir"
