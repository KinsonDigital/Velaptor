import type {
	ScriptTask,
	SprocketConfig,
} from "jsr:@kinsondigital/sprocket@3.0.0/configuration";

const config: SprocketConfig = {
	jobs: [{
		name: "Prep-Prev-Release",
		description: "Prepares for preview release.",
		preExecuteMsg: "Starting release preparation.",
		preExecuteMsgColor: "cyan",
		postExecuteMsg: "Release preparation complete.",
		postExecuteMsgColor: "cyan",
		tasks: [{
			type: "script",
			name: "Develop Branch Checked Out",
			description: "Prepares for a preview release.",
			preExecuteMsg: "\t⏳Process running. . .",
			preExecuteMsgColor: "gray",
			script: { filePath: `${Deno.cwd()}/DevTools/prep-prev-release.ts` },
		} as ScriptTask],
	}, {
		name: "Create-PR",
		description: "Creates a feature branch and pull request.",
		preExecuteMsg: "Starting Feature PR creation.",
		preExecuteMsgColor: "cyan",
		postExecuteMsg: "Feature branch and pull request created.",
		postExecuteMsgColor: "cyan",
		tasks: [{
			type: "script",
			name: "Create Feature Pull Request",
			description: "Creates a feature branch and pull request.",
			preExecuteMsg: "\t⏳Creating. . .",
			preExecuteMsgColor: "gray",
			script: { filePath: `${Deno.cwd()}/DevTools/create-pr.ts` },
		} as ScriptTask],
	}],
};

export { config };
