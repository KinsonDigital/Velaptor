<# DESCRIPTION:
# This script will create a local nuget package for the Velaptor project and place it in the specified
# output directory. It first builds the project, then removes any existing Velaptor nuget packages from
# the local source directory, and finally creates the new nuget package.

# PURPOSE: This is to easily create a local nuget package for the purpose of testing Velaptor in
# other projects without having to publish it to a remote nuget repository.
#>

param (
    [string]$BuildConfig,
    [string]$EnableTelemetry
)

if ($BuildConfig -eq $null -or $BuildConfig -eq "") {
    Write-Error "A build configuration parameter must be specified."
    exit 1;
}

if ($EnableTelemetry -eq $null -or $EnableTelemetry -eq "") {
    Write-Error "A enable telemetry parameter must be specified."
    exit 1;
}

Clear-Host;

$solutionPath = Get-Location;
$projectName = "Velaptor";
$outputPath = "S:/LocalNugetSource";
$projectPath = "$solutionPath/$projectName/$projectName.csproj";

# Build the project
dotnet build $projectPath -c $BuildConfig -o $outputPath;

# Remove all Velaptor nuget packages from the local source directory
Get-ChildItem -Path $outputPath -Filter "KinsonDigital.Velaptor.*" | Remove-Item -Force

# Create the nuget package
Write-Host "Creating nuget package using the '$BuildConfig' configuration..." -ForegroundColor Cyan;
dotnet pack $projectPath -c $BuildConfig -o $outputPath -p:EnableTelemetry=$EnableTelemetry;
