# Requires -Version 5.1
<#
.SYNOPSIS
    Routes the GitHub Copilot extension to the DeepSeek API endpoint.
.DESCRIPTION
    Run this by dot-sourcing: . .\set-copilot-deepseek.ps1
#>

# Check for a pre-existing machine-level key first to protect your credentials
$ApiKey = $env:DEEPSEEK_API_KEY;

if ([string]::IsNullOrWhiteSpace($ApiKey)) {
    # Fallback if you haven't set a machine-level variable yet
    $ApiKey = "your-deepseek-api-key-here";
}

if ($ApiKey -eq "your-deepseek-api-key-here") {
    Write-Host "⚠️ Warning: You are using the placeholder API key. Update the script or set `$env:DEEPSEEK_API_KEY." -ForegroundColor Orange;
}

# Inject the configurations into the current process environment
$env:COPILOT_MODEL = "deepseek-v4-pro";
$env:COPILOT_API_URL = "https://api.deepseek.com/v1";
$env:COPILOT_PROVIDER_TYPE = "openai";
$env:GITHUB_TOKEN = $ApiKey;

Write-Host "`nRunning GitHub Copilot with 'deepseek-v4-pro!" -ForegroundColor Green;
copilot
