#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Regenerate schema models and meta definitions from ACP schema files.

.DESCRIPTION
    This script orchestrates the code generation process:
    1. Optionally downloads schema.json and meta.json from upstream
    2. Generates C# models from schema.json
    3. Generates Meta.cs with method mappings from meta.json
    4. Applies formatting and post-processing

.PARAMETER Version
    Git ref (tag/branch) to fetch schema from. If omitted, uses cached schema files.
    Example: "v0.10.8" or "main"

.PARAMETER Repo
    Source repository providing schema files (default: agentclientprotocol/agent-client-protocol)

.PARAMETER NoDownload
    Skip downloading schema files even when a version is provided.

.PARAMETER Force
    Force schema download even if the requested ref is already cached.

.EXAMPLE
    ./gen_all.ps1 -Version "v0.10.8"
    Downloads and generates code for ACP schema v0.10.8

.EXAMPLE
    ./gen_all.ps1 -NoDownload
    Generates code using existing local schema files
#>

param(
    [string]$Version = $env:ACP_SCHEMA_VERSION,
    [string]$Repo = $(if ($env:ACP_SCHEMA_REPO) { $env:ACP_SCHEMA_REPO } else { "agentclientprotocol/agent-client-protocol" }),
    [switch]$NoDownload,
    [switch]$Force
)

$ErrorActionPreference = "Stop"

# ============================================================================
# Helper Functions
# ============================================================================

function Resolve-Ref {
    param([string]$Version)

    if (-not $Version) {
        return "refs/heads/main"
    }

    if ($Version -match '^refs/') {
        return $Version
    }

    if ($Version -match '^v?\d+\.\d+\.\d+$') {
        if ($Version -notmatch '^v') {
            $Version = "v$Version"
        }
        return "refs/tags/$Version"
    }

    return "refs/heads/$Version"
}

function Get-CachedRef {
    if (Test-Path $VersionFile) {
        return (Get-Content $VersionFile).Trim()
    }
    return $null
}

function Download-Schema {
    param(
        [string]$Repository,
        [string]$Ref
    )

    $baseUrl = "https://raw.githubusercontent.com/$Repository/$Ref/schema"
    $schemaUrl = "$baseUrl/schema.json"
    $metaUrl = "$baseUrl/meta.json"

    New-Item -ItemType Directory -Path $SchemaDir -Force | Out-Null

    try {
        Write-Host "  Fetching from: $Repository@$($Ref.Replace('refs/tags/', '').Replace('refs/heads/', ''))" -ForegroundColor Gray

        $schemaJson = (Invoke-WebRequest -Uri $schemaUrl -ErrorAction Stop).Content
        $metaJson = (Invoke-WebRequest -Uri $metaUrl -ErrorAction Stop).Content

        [System.IO.File]::WriteAllText($SchemaJsonPath, $schemaJson, [System.Text.Encoding]::UTF8)
        [System.IO.File]::WriteAllText($MetaJsonPath, $metaJson, [System.Text.Encoding]::UTF8)
        [System.IO.File]::WriteAllText($VersionFile, $Ref, [System.Text.Encoding]::UTF8)

        Write-Host "  ✓ Schema and meta files downloaded" -ForegroundColor Gray
    }
    catch {
        Write-Error "Failed to fetch schema from $Repository@$Ref : $_" -ErrorAction Stop
    }
}

# ============================================================================
# Main Script
# ============================================================================

# Setup paths
$ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProtocolDir = Split-Path -Parent $ScriptRoot
$RepoRoot = Split-Path -Parent $ProtocolDir
$SchemaDir = Join-Path $ProtocolDir "schema"
$SchemaJsonPath = Join-Path $SchemaDir "schema.json"
$MetaJsonPath = Join-Path $SchemaDir "meta.json"
$VersionFile = Join-Path $SchemaDir "VERSION"

Write-Host "🚀 ACP Code Generation" -ForegroundColor Cyan
Write-Host "Repository root: $RepoRoot" -ForegroundColor Gray
Write-Host "Schema directory: $SchemaDir" -ForegroundColor Gray

# Determine if we should download schema
$shouldDownload = $false

if ($NoDownload) {
    $shouldDownload = $false
} elseif ($Version) {
    if ((Test-Path $SchemaJsonPath) -and (Test-Path $MetaJsonPath) -and -not $Force) {
        $cachedRef = Get-CachedRef
        $targetRef = Resolve-Ref $Version
        if ($cachedRef -eq $targetRef) {
            $shouldDownload = $false
        } else {
            $shouldDownload = $true
        }
    } else {
        $shouldDownload = $true
    }
} else {
    $shouldDownload = (Test-Path $SchemaJsonPath) -and (Test-Path $MetaJsonPath)
}

# Download schema files if needed
if ($shouldDownload) {
    Write-Host "📥 Downloading schema files..." -ForegroundColor Blue
    $ref = Resolve-Ref $Version
    Download-Schema $Repo $ref
} else {
    Write-Host "📚 Using existing schema files" -ForegroundColor Blue
}

# Validate schema files exist
if (-not (Test-Path $SchemaJsonPath)) {
    Write-Error "schema/schema.json not found. Run with -Version to download." -ErrorAction Stop
}
if (-not (Test-Path $MetaJsonPath)) {
    Write-Error "schema/meta.json not found. Run with -Version to download." -ErrorAction Stop
}

# Generate schema models
Write-Host "⚙️  Generating C# models from schema..." -ForegroundColor Blue
& (Join-Path $ScriptRoot "gen_schema.ps1")

# Generate meta definitions
Write-Host "⚙️  Generating meta definitions..." -ForegroundColor Blue
& (Join-Path $ScriptRoot "gen_meta.ps1")

Write-Host "✅ Code generation completed successfully!" -ForegroundColor Green

if ($shouldDownload) {
    $ref = Get-CachedRef
    Write-Host "Generated using ref: $ref" -ForegroundColor Gray
}

