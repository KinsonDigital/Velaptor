import { isNothing } from "./guards.ts";

/**
 * Get the value of an environment variable after checking if it exists.
 * @param name The name of the environment variable.
 * @remarks This function will throw an error if the environment variable does not exist.
 */
export function getEnvVar(
	name: string,
	scriptFileName?: string,
	throwErrorIfMissing: boolean = true,
): string {
	const value = (Deno.env.get(name) ?? "").trim();

	if (isNothing(value) && throwErrorIfMissing) {
		const fileName = isNothing(scriptFileName) ? "" : `\n\t${scriptFileName}`;
		const errorMsg = `The '${name}' environment variable does not exist.${fileName}`;
		console.log(`::error::${errorMsg}`);

		Deno.exit(1);
	}

	return value;
}
