#!/bin/bash

# Throw an error if any variables are not set
set -u

print_param_help() {
    echo "Param 1: build-config=<debug|release>"
    echo "Param 2: enable-telemetry=<true|false>"
}

# Check if the 'build-config' parameter is an empty string or does not exist
if [ -z "${1:-}" ]; then
    echo "No parameters exist. Must pass the following parameters."
    print_param_help

    exit 1
fi

# Remove all spaces
buildConfigParam=$(echo "$1" | tr -d ' ')
buildConfigParam="${buildConfigParam,,}" # set all to lowercase

# The value must be 'Debug' or 'Release'
if [[ $buildConfigParam != 'build-config=debug' && $buildConfigParam != 'build-config=release' ]]; then
    print_param_help

    exit 1
fi

# Split the first argument into parts
IFS="=" read -ra parts <<< "$1"

# Get the build config
buildConfig="${parts[1]}"

# Check if the 'enable-telemetry' parameter is an empty string or does not exist
if [ -z "${2:-}" ]; then
    echo "The second parameter 'enable-telemetry' does not exist. Must pass the following parameters."
    print_param_help

    exit 1
fi

# Remove all spaces
enableTelemetryParam=$(echo "$2" | tr -d ' ')
enableTelemetryParam="${enableTelemetryParam,,}" # set all to lowercase

# The value must be 'true' or 'false'
if [[ $enableTelemetryParam != 'enable-telemetry=true' && $enableTelemetryParam != 'enable-telemetry=false' ]]; then
    print_param_help

    exit 1
fi

# If the telemetry key variable does not exist.
if [ -z "${TELEMETRY_KEY:-}" ]; then
    echo "The environment variable 'TELEMETRY_KEY' does not exist, or is empty."

    exit 1
fi

# Split the second argument into parts
IFS="=" read -ra parts <<< "$2"

# Get the telemetry enable state
enableTelemetry="${parts[1]}"

# Check if dotnet is not installed
DOTNET_PATH="$(command -v dotnet)"
if [ -z "$DOTNET_PATH" ]; then
    echo "The dotnet SDK must be installed first."
    echo "Install dotnet: https://dotnet.microsoft.com/en-us/download"

    exit 1
fi

currentWorkingDir=$(pwd)
projectName="Velaptor"
outputPath="$HOME/software-development/LocalNugetSource"
projectPath="$currentWorkingDir/$projectName/$projectName.csproj"

# Create the output path if it does not exist
if [ ! -d "${path:-}" ]; then
    mkdir -p "$outputPath"
fi

# Build the project
dotnet build "$projectPath" -c $buildConfig -o "$outputPath"

# Delete all files that are not nuget packages
find "$outputPath" -maxdepth 1 -type f -not -name "*.nupkg" -delete

# Create the nuget package
echo "\nCreating nuget package using the '$buildConfig' configuration...\n"
dotnet pack "$projectPath" -c $buildConfig -o "$outputPath" -p:EnableTelemetry=$enableTelemetry -p:TelemetryKey=$TELEMETRY_KEY
