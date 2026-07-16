import { existsSync, walkSync } from "@std/fs";
import { TagClient } from "@kd-clients/github";
import { Select } from "@cliffy/prompt";
import { printGitHubError, printGitHubNotice } from "./core/github-actions.ts";
import { isNothing } from "./core/guards.ts";

const rootDirPath = (Deno.env.get("ROOT_DIR_PATH") ?? "").trim().replaceAll("\\", "/");

if (rootDirPath === "") {
	const errorMsg = "The environment variable 'ROOT_DIR_PATH' does not exist.";
	printGitHubError(errorMsg);

	Deno.exit(1);
}

// If the directory does not exist, throw and error
if (!existsSync(rootDirPath, { isDirectory: true })) {
	console.log(`::error::The given directory path '${rootDirPath}' does not exist.`);

	Deno.exit(200);
}

const tagRegex = /^v([1-9]\d*|0)\.([1-9]\d*|0)\.([1-9]\d*|0)(-preview\.([1-9]\d*))?$/;

const possibleVersion = (Deno.env.get("VERSION_OR_INTERACTIVE") ?? "").trim().toLowerCase();

if (possibleVersion === "") {
	const errorMsg = "The environment variable 'VERSION_OR_INTERACTIVE' does not exist.";
	printGitHubError(errorMsg);

	Deno.exit(1);
}

const isInteractive = possibleVersion === "interactive";

let newVersion = "";

if (isInteractive) {
	const token = (Deno.env.get("GITHUB_TOKEN") ?? "").trim();

	if (token === "") {
		const errorMsg = "The environment variable 'GITHUB_TOKEN' does not exist.";
		printGitHubError(errorMsg);

		Deno.exit(1);
	}

	const tagClient = new TagClient("KinsonDigital", "Velaptor", token);

	const tags = (await tagClient.getAllTags()).map((tag) => tag.name);

	newVersion = await Select.prompt({
		message: "Select a Velaptor version",
		options: tags,
		validate: (value: string) => tagRegex.test(value),
	});

	console.log(`The tag selected was: ${newVersion}`);
} else {
	newVersion = possibleVersion;
}

newVersion = newVersion.startsWith("v") ? newVersion.substring(1) : newVersion;

const newNugetPackage = `<PackageReference Include="KinsonDigital.Velaptor" Version="${newVersion}" />`;

const velaptorNuGetRegex =
	/<PackageReference\s+Include\s*=\s*\"KinsonDigital.Velaptor\"\s+Version\s*=\s*\"([1-9]\d*|0)\.([1-9]\d*|0)\.([1-9]\d*|0)(-preview\.([1-9]\d*))?\"\s*\/>/;

// Get all the csproj files
const projEntries = walkSync(rootDirPath, {
	includeDirs: false,
	includeFiles: true,
	exts: [".csproj"],
});

const csprojFiles = [...projEntries].map((entry) => entry.path);

// Replace the nuget package reference with the new version
csprojFiles.forEach((file) => {
	const fileData = Deno.readTextFileSync(file);

	const nugetRefs = velaptorNuGetRegex.exec(fileData)?.map((match) => match.toString()) ?? [];

	const velaptorNuGetRef = nugetRefs.length >= 0 ? nugetRefs[0] : "";
	const containsVelaptorNuGetRef = !isNothing(velaptorNuGetRef);

	// If the file contains the nuget package
	if (containsVelaptorNuGetRef) {
		const versionRegex = /([1-9]\d*|0)\.([1-9]\d*|0)\.([1-9]\d*|0)(-preview\.([1-9]\d*))?/;
		const oldVersion = versionRegex.exec(velaptorNuGetRef)?.map((match) => match.toString())[0] ?? "";

		const newFileData = fileData.replace(velaptorNuGetRegex, newNugetPackage);

		Deno.writeTextFileSync(file, newFileData);

		const updateFileMsg = `The NuGet package 'KinsonDigital.Velaptor' was updated from version` +
			`'${oldVersion}' to version '${newVersion}' in the csproj file '${file}`;
		printGitHubNotice(updateFileMsg);
	}
});
