param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    [switch]$EnableModernTargetFrameworks = $true
)

$ErrorActionPreference = "Stop"

$props = @()
if ($EnableModernTargetFrameworks) {
    $props += "/p:EnableCiTargetFrameworks=true"
}

Write-Host "Restoring solution..."
dotnet restore "dotacp.sln"

Write-Host "Building solution..."
dotnet build "dotacp.sln" -c $Configuration --no-restore $props

Write-Host "Running unit tests..."
dotnet test "unittest/unittest.csproj" -c $Configuration --no-build

Write-Host "CI build and tests completed successfully."
