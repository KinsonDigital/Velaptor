import { existsSync, walkSync } from "@std/fs";
import { printGitHubError, printGitHubNotice } from "./core/github-actions.ts";
import { getEnvVar } from "./core/utils.ts";

// Permissions: -ERW

const scriptName = `\n\tScript Name : ${import.meta.url.split("/").pop()}`;

const searchDirPath = getEnvVar("SEARCH_DIR_PATH", scriptName);
const newVersion = getEnvVar("NEW_VERSION", scriptName);

// If the base dir path does not exist
if (!existsSync(searchDirPath)) {
	printGitHubError(`The 'CS_PROJ_FILE_PATH' does not exist.`);

	Deno.exit(1);
}

const searchResults = Array.from(walkSync(searchDirPath, { match: [/Velaptor.csproj/] }))
	.map((e) => e.path);

// If the file was not found
if (searchResults.length <= 0) {
	printGitHubError(`The dotnet project file 'Velaptor.csproj' was not found.`);

	Deno.exit(1);
}

const csProjFilePath = searchResults[0];

const newVersionRegex = /^([1-9]\d*|0)\.([1-9]\d*|0)\.([1-9]\d*|0)(-preview\.([1-9]\d*))?$/;

if (newVersionRegex.test(newVersion)) {
	printGitHubError(`The version '${newVersion}' is invalid.  Must be of type '#.#.#[-preview.#]'.`);

	Deno.exit(1);
}

const versionRegex = /<Version>([1-9]\d*|0)\.([1-9]\d*|0)\.([1-9]\d*|0)(-preview\.([1-9]\d*))?<\/Version>/gm;
const fileVersionRegex = /<FileVersion>([1-9]\d*|0)\.([1-9]\d*|0)\.([1-9]\d*|0)(-preview\.([1-9]\d*))?<\/FileVersion>/gm;

// Update the versions for both the '<Version/>' and '<FileVersion/>' values in the csproj file.
try {
	let fileContent = Deno.readTextFileSync(csProjFilePath);

	fileContent = fileContent.replace(versionRegex, `<Version>${newVersion}</Version>`);
	fileContent = fileContent.replace(fileVersionRegex, `<FileVersion>${newVersion}</FileVersion>`);

	Deno.writeTextFileSync(csProjFilePath, fileContent);

	printGitHubNotice(`Updated the '${csProjFilePath}' version to version '${newVersion}'.`);
} catch (error) {
	const errorMsg = error instanceof Error ? error.message : String(error);

	printGitHubError(`Error processing csproj project file '${csProjFilePath}'\n\t${errorMsg}`);

	Deno.exit(1);
}
