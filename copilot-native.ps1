# Requires -Version 5.1
<#
.SYNOPSIS
    Restores the native GitHub Copilot backend for the current terminal session.
.DESCRIPTION
    Run this by dot-sourcing: . .\set-copilot-native.ps1
#>

# Clear the environment variables from the current process scope
$env:COPILOT_PROVIDER_BASE_URL = $null;
$env:COPILOT_PROVIDER_API_KEY = $null;
$env:COPILOT_MODEL = $null;
$env:COPILOT_PROVIDER_TYPE = $null;

Write-Host "`nRunning native GitHub Copilot" -ForegroundColor Green;
copilot
