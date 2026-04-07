import { delay } from "jsr:@std/async@1.0.15";
import { existsSync, walkSync } from "jsr:@std/fs@1.0.19";
import { Input } from "jsr:@cliffy/prompt@1.0.0-rc.8/input";
import { IssueOrPRRequestData } from "jsr:@kinsondigital/kd-clients@1.0.0-preview.15/core";
import {
	IssueClient,
	ProjectClient,
	PullRequestClient,
} from "jsr:@kinsondigital/kd-clients@1.0.0-preview.15/github";
import {
	branchExistsLocally,
	branchExistsRemotely,
	checkoutBranch,
	createCheckoutBranch,
	createCommit,
	isCheckedOut,
	pushToRemote,
} from "jsr:@kinsondigital/sprocket@2.2.0/git";
import {
	printCyan,
	printGray,
	printIndianRed,
	printYellow,
} from "jsr:@kinsondigital/sprocket@2.2.0/console";

const token = (Deno.env.get("CICD_TOKEN") ?? "").trim();
const prReviewer = "KinsonDigitalAdmin";

if (token === undefined || token === null || token === "") {
	console.log("The environment variable 'CICD_TOKEN' does not exist.");
	Deno.exit(1);
}

const GET_DIR_PATH = "./.git";
const GIT_CONFIG_FILE_PATH = "./.git/config";

// If the git dir path or git config file path do not exist, notify the user and stop the process
if (
	!existsSync(GET_DIR_PATH, { isDirectory: true }) ||
	!existsSync(GIT_CONFIG_FILE_PATH, { isFile: true })
) {
	printIndianRed("Not a valid git repository");

	Deno.exit(1);
}

let repoOwnerName = "";
let repoName = "";

try {
	printGray("Validating repository");
	const gitConfigFileData = Deno.readTextFileSync(GIT_CONFIG_FILE_PATH);
	const remoteOriginMatch =
		gitConfigFileData.match(/\[remote "origin"\][\s\S]*?url = (.+)/m) ?? "";

	if (remoteOriginMatch === null) {
		printIndianRed("The repository does not have a remote configured.");

		Deno.exit(1);
	}

	const remoteText = remoteOriginMatch[0];

	const urlRegex = /url = (.+)/;
	const urlMatch = remoteText.match(urlRegex);

	if (urlMatch === null) {
		printIndianRed(
			"The repository does not have a remote 'origin' URL configured.",
		);

		Deno.exit(1);
	}

	const urlMatchText = urlMatch[0];
	let url = urlMatchText.split("=")[1].trim();
	const githubUrlRegex = /https:\/\/github\.com\/(.+\/)(.+)\.git/;

	if (!githubUrlRegex.test(url)) {
		printIndianRed("The remote 'origin' URL is not a valid GitHub URL.");
		Deno.exit(1);
	}

	printGray("Getting GitHub repository owner and name");
	url = url.replace("https://github.com/", "").replace(".git", "");
	const urlSections = url.split("/");
	[repoOwnerName, repoName] = urlSections;

	printGray(`Repository owner ${repoOwnerName}`);
	printGray(`Repository name ${repoName}`);
} catch (error) {
	const errMsg = error instanceof Error
		? error.message
		: "An error occurred reading the git config file.";
	printIndianRed(errMsg);

	Deno.exit(1);
}

// Validates that the issue number exists
const issueClient = new IssueClient(repoOwnerName, repoName, token);

// Ask the user for an issue number
const issueNumberResult = await Input.prompt({
	message: "Enter the issue number:",
	info: true,
	validate: async (value: string) => {
		if (isNaN(Number(value))) {
			return "Issue number must be a valid number.";
		}

		if (value.includes(".")) {
			return "The issue number must be a whole number.";
		}

		if (Number(value) <= 0) {
			return "The issue number must be greater than zero.";
		}

		try {
			const issueExists = await issueClient.exists(Number(value));

			if (!issueExists) {
				return `The issue '${value}' does not exist.`;
			}
		} catch {
			return `The issue number '${value}' does not exist or there was an error fetching it.`;
		}

		return true;
	},
});

const issueNumber = Number(issueNumberResult);

// Ask the user for a branch name description
const featureBranch = await Input.prompt({
	message: "Enter the branch name:",
	hint: "my feature branch",
	validate: (value: string) => {
		if (value.trim() === "") {
			return "Branch name cannot be empty.";
		}

		const regex = /^[a-zA-Z\-]+$/gm;
		// Remove leading and trailing hyphens, trim, lowercase, and replace spaces and underscores with hyphens
		value = value.trim().toLowerCase()
			.replaceAll(" ", "-")
			.replaceAll("_", "-")
			.replace(/-{2,}/g, "-")
			.replace(/^-+/, "")
			.replace(/-+$/, "");

		if (!regex.test(value)) {
			printIndianRed(
				"Branch name must match the pattern 'feature/123-my-branch'.",
			);

			return false;
		}

		return true;
	},
	transform: (value) => {
		const transformedValue = value.trim().toLowerCase()
			.replaceAll(" ", "-")
			.replaceAll("_", "-")
			.replace(/-{2,}/g, "-") // 2 or more consecutive hyphens
			.replace(/^-+/, "")
			.replace(/-+$/, "");

		return `feature/${issueNumber}-${transformedValue}`;
	},
});

const chosenBaseBranch = "preview";

// If the chosen branch exists
if (await branchExistsLocally(chosenBaseBranch)) {
	// If the chosen base branch is not checked out, checkout the branch
	if (!(await isCheckedOut(chosenBaseBranch))) {
		printGray(`Checking out the local branch '${chosenBaseBranch}'.`);
		await checkoutBranch(chosenBaseBranch);
	}
} else if (await branchExistsRemotely(chosenBaseBranch)) {
	printGray(`Checking out the remote branch '${chosenBaseBranch}'.`);
	await checkoutBranch(chosenBaseBranch);
} else {
	printGray(`Creating and checking out the branch '${chosenBaseBranch}'.`);
	await createCheckoutBranch(chosenBaseBranch);
	printGray(`Pushing the branch '${chosenBaseBranch}' to remote.`);
	await pushToRemote(chosenBaseBranch);
}

try {
	// Create a branch using git commands from the currently checked out branch.
	printGray(`Creating branch '${featureBranch}'`);
	await createCheckoutBranch(featureBranch);

	// Create an empty commit
	printGray("Creating empty start commit");
	await createCommit(`Start work for issue #${issueNumber}`, true);

	// Push the branch to remote
	printGray(`Pushing branch to remote`);
	await pushToRemote(featureBranch);

	// Delay for 1 second to allow GitHub to finalize the branch creation
	await delay(1000);

	const issue = await issueClient.getIssue(issueNumber);

	printGray("Searching for 'pr-template.md' file");
	const templateFiles = Array.from(
		walkSync("./", { includeFiles: true, match: [/pr-template\.md$/] }),
	)
		.map((entry) => entry.path);

	const prTemplateFilePath = templateFiles.length > 0 ? templateFiles[0] : "";
	const noTemplateFoundDescription =
		"No template file 'pr-template.md' was found.";
	const templateFound = prTemplateFilePath !== "";

	if (templateFound) {
		printGray(`Found pull request template at '${prTemplateFilePath}'`);
	} else {
		printGray(noTemplateFoundDescription);
	}

	let prDescription = templateFound
		? await Deno.readTextFile(prTemplateFilePath)
		: noTemplateFoundDescription;

	// Replace issue number placeholder with actual issue number
	prDescription = prDescription.replace(
		"{ISSUE_NUMBER}",
		issue.number.toString(),
	);

	// Create a pull request
	const prClient = new PullRequestClient(repoOwnerName, repoName, token);

	printGray("Creating pull request");
	const newPr = await prClient.createPullRequest(
		issue.title ?? "WIP - Please update title",
		featureBranch,
		chosenBaseBranch,
		prDescription,
		true,
		true,
	);

	printGray(`Setting pull request reviewer to '${prReviewer}'`);
	await prClient.requestReviewers(newPr.number, prReviewer);

	const prData: IssueOrPRRequestData = {
		labels: issue.labels.map((l) => l.name),
		assignees: issue.assignees.map((a) => a.login),
		milestone: issue.milestone?.number,
	};

	// Update the labels assignees, and milestone to match the linked issue
	printGray(
		`Setting pull request assignees, labels, and milestone to match issue '${newPr.number}'.`,
	);
	await prClient.updatePullRequest(newPr.number, prData);

	const projClient = new ProjectClient(repoOwnerName, repoName, token);

	printGray("Getting issue projects");
	const issueProjects = await projClient.getIssueProjects(issueNumber);

	// Sync all of the issue projects to the pull request
	for await (const issueProject of issueProjects) {
		printGray(`Adding pull request to project '${issueProject.title}'`);
		await projClient.addPullRequestToProject(
			newPr.number,
			issueProject.title,
		);
	}

	printCyan(`Pull request '#${newPr.number}' has been created successfully!`);
	printCyan(`URL: ${newPr.html_url}`);
} catch (error) {
	const errMsg = error instanceof Error
		? error.message
		: "An error occurred.";
	printIndianRed(errMsg);

	printYellow("\nCheck the following fine-grained access token permissions:");
	printYellow("\tRepo Permissions:");
	printYellow("\t  %cContents: read & write");
	printYellow("\t  %cIssues: read only");
	printYellow("\t  %cMetadata: read only");
	printYellow("\t  %cPull requests: read & write");
	printYellow("\tOrg Permissions:");
	printYellow("\t  %cProjects: read & write");
}
