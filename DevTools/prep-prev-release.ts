import { existsSync } from "jsr:@std/fs@1.0.19";
import { Input } from "jsr:@cliffy/prompt@1.0.0-rc.8";
import {
	branchExistsLocally,
	branchExistsRemotely,
	checkoutBranch,
	createCheckoutBranch,
	createCommit,
	isCheckedOut,
	noUncommittedChangesExist,
	pushToRemote,
	stageFiles,
	uncommittedChangesExist,
} from "jsr:@kinsondigital/sprocket@2.1.0/git";
import {
	LabelClient,
	MilestoneClient,
	ProjectClient,
	PullRequestClient,
} from "jsr:@kinsondigital/kd-clients@1.0.0-preview.15";
import { IssueOrPRRequestData } from "jsr:@kinsondigital/kd-clients@1.0.0-preview.15/core";
import { printGray } from "jsr:@kinsondigital/sprocket@2.1.0/console";
import { ReleaseNotesGenerator } from "https://jsr.io/@kinsondigital/sprocket/2.2.0/src/release-notes-generator.ts";
import { GeneratorSettings } from "https://jsr.io/@kinsondigital/sprocket/2.2.0/src/core/releases.ts";

const token = (Deno.env.get("CICD_TOKEN") ?? "").trim();

if (token === "") {
	console.log("The environment variable 'CICD_TOKEN' is required.");
	Deno.exit(1);
}

const projectName = "Velaptor";
const projFileName = `${projectName}.csproj`;
const csProjFilePath = `./Velaptor/${projFileName}`;
const projectFileData = Deno.readTextFileSync(csProjFilePath);
const versionRegex = /<Version>(.+)<\/Version>/;
const fileVersionRegex = /<FileVersion>(.+)<\/FileVersion>/;
const versionMatch = projectFileData.match(versionRegex);
const fileVersionMatch = projectFileData.match(fileVersionRegex);

if (versionMatch === null) {
	console.log("Could not find version in the .csproj file.");
	Deno.exit(1);
}

if (fileVersionMatch === null) {
	console.log("Could not find file version in the .csproj file.");
	Deno.exit(1);
}

const ownerName = "KinsonDigital";
const repoName = "Velaptor";
const prevLabel = "🚀preview-release";
const baseBranch = "preview";
const releaseType = "Preview";

// Ask the user for a version number
const releaseVersion = await Input.prompt({
	message: "Enter the release version:",
	validate: (value) => {
		const prevVersionRegex =
			/^v([1-9]\d*|0)\.([1-9]\d*|0)\.([1-9]\d*|0)-preview\.([1-9]\d*)$/gm;

		return prevVersionRegex.test(value.trim().toLowerCase());
	},
	transform: (value) => {
		const result = value.trim().toLowerCase();

		return result.startsWith("v") ? result.slice(1) : result;
	},
});

printGray(`⌛Validating the label '${prevLabel}'. . .`);
const labelClient = new LabelClient(ownerName, repoName, token);
const labelExists = await labelClient.exists(prevLabel);

if (!labelExists) {
	console.error(
		`The label '${prevLabel}' does not exist in the repository '${ownerName}/${repoName}'.`,
	);
	Deno.exit(1);
}

const settingsFileName = "prev-gen-release-notes-settings.json";
const settingsFilePath = `${Deno.cwd()}/DevTools/${settingsFileName}`;

if (!existsSync(settingsFilePath)) {
	console.error(
		`The release notes settings file '${settingsFileName}' does not exist.`,
	);
	Deno.exit(1);
}

printGray(`⌛Checking if the branch '${baseBranch}' exists locally. . .`);
// Check if the main branch is checked out
if (await branchExistsLocally(baseBranch)) {
	// If the base branch is checked out
	if (await isCheckedOut(baseBranch)) {
		if (await uncommittedChangesExist()) {
			console.log(
				`You have uncommitted changes in your working directory. Please commit or stash them before preparing a release.`,
			);
			Deno.exit(1);
		}
	} else {
		if (await noUncommittedChangesExist()) {
			await checkoutBranch(baseBranch);
		} else {
			console.log(
				`You have uncommitted changes in your working directory. Please commit or stash them before preparing a release.`,
			);
			Deno.exit(1);
		}
	}
} else {
	printGray(`⌛Checking if the branch '${baseBranch}' exists remotely. . .`);
	if (await branchExistsRemotely(baseBranch)) {
		await checkoutBranch(baseBranch);
	} else {
		console.log(
			`The base branch '${baseBranch}' does not exist locally or remotely. Please create it before preparing a release.`,
		);
		Deno.exit(1);
	}
}

const headBranch = `${releaseType.toLowerCase()}-release`;

printGray(`⌛Creating the branch '${headBranch}'. . .`);
await createCheckoutBranch(headBranch);

printGray(`⌛Updating the version in the '${csProjFilePath}' file. . .`);
const updatedProjectFileData = projectFileData.replace(
	versionRegex,
	`<Version>${releaseVersion}</Version>`,
).replace(
	fileVersionRegex,
	`<FileVersion>${releaseVersion}</FileVersion>`,
);
Deno.writeTextFileSync(csProjFilePath, updatedProjectFileData);

printGray("⌛\tStaging version changes. . .");
await stageFiles([`*${projFileName}`]);
printGray("⌛\tCreating commit for version changes. . .");

// If there are changes to commit
if (await uncommittedChangesExist()) {
	await createCommit(`release: update version to v${releaseVersion}`);
}

printGray("⌛Generating release notes. . .");
const releaseNotesFileName = `Release-Notes-v${releaseVersion}.md`;
const releaseNotesFilePath =
	`${Deno.cwd()}/ReleaseNotes/${releaseType}Releases/${releaseNotesFileName}`;
const generator: ReleaseNotesGenerator = new ReleaseNotesGenerator();
const settingsFileContent = Deno.readTextFileSync(settingsFilePath);
const settings: GeneratorSettings = JSON.parse(settingsFileContent);
settings.version = `v${releaseVersion}`;

const notes = await generator.generateNotes(settings);
Deno.writeTextFileSync(releaseNotesFilePath, notes);

printGray("⌛\tStaging release note changes. . .");
await stageFiles([`*${releaseNotesFileName}`]);
printGray("⌛\tCreating commit for release note changes. . .");
await createCommit(
	`release: create release notes for version v${releaseVersion}`,
);

printGray("⌛Pushing changes to remote. . .");
await pushToRemote(headBranch);

const title = `🚀Preview Release (v${releaseVersion})`;
const assignee = "CalvinWilkinson";
const githubProjectName = "KD-Team";
const reviewer = "KinsonDigitalAdmin";

const prevReleasePrTemplateFilePath =
	`${Deno.cwd()}/templates/prev-prepare-release-template.md`;
const templateFileContent = Deno.readTextFileSync(
	prevReleasePrTemplateFilePath,
);

printGray(`⌛Getting milestone data. . .`);
const milestoneClient = new MilestoneClient(ownerName, repoName, token);
const milestone = await milestoneClient.getMilestoneByName(
	`v${releaseVersion}`,
);

printGray(
	`⌛Creating pull request to merge the branch '${headBranch}' into the branch '${baseBranch}'. . .`,
);
const prClient = new PullRequestClient(ownerName, repoName, token);
const newPr = await prClient.createPullRequest(
	title,
	headBranch,
	baseBranch,
	templateFileContent,
);

printGray(`⌛Setting the pull request reviewer to '#${reviewer}'. . .`);
await prClient.requestReviewers(newPr.number, [reviewer]);

printGray(
	`⌛Updating pull request '#${newPr.number}' assignee, label, and milestone. . .`,
);
const prData: IssueOrPRRequestData = {
	assignees: [assignee],
	labels: [prevLabel],
	milestone: milestone.number,
};

await prClient.updatePullRequest(newPr.number, prData);

printGray(
	`⌛Adding pull request '#${newPr.number}' to project '${githubProjectName}'. . .`,
);
const projClient = new ProjectClient(ownerName, repoName, token);

await projClient.addPullRequestToProject(newPr.number, githubProjectName);

const prUrl =
	`https://github.com/${ownerName}/${repoName}/pull/${newPr.number}`;
console.log(`Pull Request: ${prUrl}`);
