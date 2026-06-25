/**
 * Description:
 * Routes the GitHub Copilot extension to use the DeepSeek provider with the "deepseek-v4-pro" model.
 */

const apiKey = Deno.env.get("DEEPSEEK_API_KEY");

if (apiKey === undefined) {
	console.error("Error: DEEPSEEK_API_KEY environment variable is not set.");
	Deno.exit(1);
}

Deno.env.set("COPILOT_MODEL", "deepseek-v4-pro");
Deno.env.set("COPILOT_PROVIDER_BASE_URL", "https://api.deepseek.com/v1");
Deno.env.set("COPILOT_PROVIDER_TYPE", "openai");
Deno.env.set("COPILOT_PROVIDER_API_KEY", apiKey);
Deno.env.set("COPILOT_PROVIDER_MAX_PROMPT_TOKENS", "840000");
Deno.env.set("COPILOT_PROVIDER_MAX_OUTPUT_TOKENS", "128000");

console.log("%c\nRunning GitHub Copilot with 'deepseek-v4-pro'!", "color: cyan; font-weight: bold;");

Deno.spawn("copilot");
