import { existsSync, walkSync } from "@std/fs";
import { printGitHubError, printGitHubNotice } from "./core/github-actions.ts";
import { isNothing } from "./core/guards.ts";
import { getEnvVar } from "./core/utils.ts";

const scriptName = `\n\tScript Name : ${import.meta.url.split("/").pop()}`;

const rootDirPath = getEnvVar("ROOT_DIR_PATH", scriptName).replaceAll("\\", "/");
let newVersion = getEnvVar("NEW_VERSION", scriptName).toLowerCase();
const nugetPkgName = getEnvVar("NUGET_PKG_NAME", scriptName);

// If the directory does not exist, throw and error
if (!existsSync(rootDirPath, { isDirectory: true })) {
	printGitHubError(`The given directory path '${rootDirPath}' does not exist.`);

	Deno.exit(1);
}

const newVersionRegex = /^([1-9]\d*|0)\.([1-9]\d*|0)\.([1-9]\d*|0)(-preview\.([1-9]\d*))?$/;

newVersion = newVersion.startsWith("v") ? newVersion.substring(1) : newVersion;

if (!newVersionRegex.test(newVersion)) {
	printGitHubError(`The version '${newVersion}' is invalid.  Must be of type '#.#.#[-preview.#]'.`);

	Deno.exit(1);
}

const newNugetPackage = `<PackageReference Include="${nugetPkgName}" Version="${newVersion}" />`;
const velaptorNuGetRegex = new RegExp(`<PackageReference\\s+Include\\s*=\\s*"${nugetPkgName}"\\s+Version\\s*=\\s*"([1-9]\\d*|0)\\.([1-9]\\d*|0)\\.([1-9]\\d*|0)(-preview\\.([1-9]\\d*))?"\\s*/>`);

// Get all the csproj files
const csprojFiles = Array.from(walkSync(rootDirPath, { includeFiles: true, exts: [".csproj"],})).map((e) => e.path);

// Replace the nuget package reference with the new version
csprojFiles.forEach((csProjFile) => {
	try {
		const fileData = Deno.readTextFileSync(csProjFile);

		const nugetRefs = velaptorNuGetRegex.exec(fileData)?.map((match) => match.toString()) ?? [];

		const velaptorNuGetRef = nugetRefs.length > 0 ? nugetRefs[0] : "";
		const containsVelaptorNuGetRef = !isNothing(velaptorNuGetRef);

		// If the file contains the nuget package
		if (containsVelaptorNuGetRef) {
			const versionRegex = /([1-9]\d*|0)\.([1-9]\d*|0)\.([1-9]\d*|0)(-preview\.([1-9]\d*))?/;
			const oldVersion = versionRegex.exec(velaptorNuGetRef)?.map((match) => match.toString())[0] ?? "";

			const newFileData = fileData.replace(velaptorNuGetRegex, newNugetPackage);

			Deno.writeTextFileSync(csProjFile, newFileData);

			const updateFileMsg = `The NuGet package 'KinsonDigital.Velaptor' was updated from version` +
				`'${oldVersion}' to version '${newVersion}' in the csproj file '${csProjFile}'`;
			printGitHubNotice(updateFileMsg);
		}
	} catch (error) {
		const errorMsg = error instanceof Error ? error.message : String(error);
		printGitHubError(`Error updating Velaptor nuget package in csproj file '${csProjFile}'.\n\t${errorMsg}`);

		Deno.exit();
	}
});
