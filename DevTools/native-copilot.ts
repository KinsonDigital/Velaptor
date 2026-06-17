/**
 * Description:
 * Routes the GitHub Copilot extension to use the native GitHub Copilot backend.
 */

Deno.env.set("COPILOT_MODEL", "");
Deno.env.set("COPILOT_PROVIDER_BASE_URL", "");
Deno.env.set("COPILOT_PROVIDER_TYPE", "");
Deno.env.set("COPILOT_PROVIDER_API_KEY", "");

console.log("%c\nRunning native GitHub Copilot!", "color: cyan; font-weight: bold;");

Deno.spawn("copilot");
