type Workflow = {
	id: number;
	name: string;
	path: string;
	state: string;
};

type WorkflowsResponse = {
	workflows: Workflow[];
};

type WorkflowRun = {
	id: number;
	status: string;
	conclusion: string | null;
	created_at: string;
	html_url: string;
	head_branch: string;
	event: string;
};

type WorkflowRunsResponse = {
	workflow_runs: WorkflowRun[];
};

type ScriptArgs = {
	owner: string;
	repo: string;
	workflow: string;
	branch: string;
	startedAfter: Date;
	maxAttempts: number;
	delaySeconds: number;
};

const terminalSuccessConclusion = "success";

const sleep = (milliseconds: number) => new Promise((resolve) => setTimeout(resolve, milliseconds));

const parseCliArgs = (): Map<string, string> => {
	const parsedArgs = new Map<string, string>();

	for (let i = 0; i < Deno.args.length; i++) {
		const arg = Deno.args[i];

		if (!arg.startsWith("--")) {
			continue;
		}

		const key = arg.slice(2);
		const nextArg = Deno.args[i + 1];

		if (nextArg === undefined || nextArg.startsWith("--")) {
			parsedArgs.set(key, "true");
			continue;
		}

		parsedArgs.set(key, nextArg);
		i++;
	}

	return parsedArgs;
};

const requiredArg = (args: Map<string, string>, name: string): string => {
	const value = args.get(name)?.trim() ?? "";

	if (value.length === 0) {
		console.error(`Missing required argument '--${name}'.`);
		Deno.exit(1);
	}

	return value;
};

const parsePositiveInt = (
	args: Map<string, string>,
	name: string,
	defaultValue: number,
): number => {
	const rawValue = args.get(name)?.trim() ?? defaultValue.toString();
	const parsedValue = Number(rawValue);

	if (!Number.isInteger(parsedValue) || parsedValue <= 0) {
		console.error(`Argument '--${name}' must be a positive whole number.`);
		Deno.exit(1);
	}

	return parsedValue;
};

const parseScriptArgs = (): ScriptArgs => {
	const args = parseCliArgs();
	const startedAfterValue = requiredArg(args, "started-after");
	const startedAfter = new Date(startedAfterValue);

	if (Number.isNaN(startedAfter.getTime())) {
		console.error("Argument '--started-after' must be a valid ISO 8601 date.");
		Deno.exit(1);
	}

	return {
		owner: requiredArg(args, "owner"),
		repo: requiredArg(args, "repo"),
		workflow: requiredArg(args, "workflow"),
		branch: requiredArg(args, "branch"),
		startedAfter: startedAfter,
		maxAttempts: parsePositiveInt(args, "max-attempts", 60),
		delaySeconds: parsePositiveInt(args, "delay-seconds", 60),
	};
};

const getToken = (): string => {
	const token = (Deno.env.get("GH_TOKEN") ?? Deno.env.get("GITHUB_TOKEN") ?? "")
		.trim();

	if (token.length === 0) {
		console.error("The 'GH_TOKEN' or 'GITHUB_TOKEN' environment variable is required.");
		Deno.exit(1);
	}

	return token;
};

const requestJson = async <T>(url: URL, token: string): Promise<T> => {
	const response = await fetch(url, {
		headers: {
			Accept: "application/vnd.github+json",
			Authorization: `Bearer ${token}`,
			"User-Agent": "KinsonDigital-Velaptor-Release-Gate",
			"X-GitHub-Api-Version": "2022-11-28",
		},
	});

	if (!response.ok) {
		const responseBody = await response.text();
		throw new Error(
			`GitHub API request failed (${response.status} ${response.statusText}): ${responseBody}`,
		);
	}

	return await response.json() as T;
};

const normalize = (value: string): string => value.trim().toLowerCase();

const getFileName = (path: string): string => {
	const parts = path.split("/");

	return parts[parts.length - 1] ?? path;
};

const findWorkflow = async (
	args: ScriptArgs,
	token: string,
): Promise<Workflow> => {
	const workflowsUrl = new URL(
		`https://api.github.com/repos/${args.owner}/${args.repo}/actions/workflows`,
	);
	workflowsUrl.searchParams.set("per_page", "100");

	const response = await requestJson<WorkflowsResponse>(workflowsUrl, token);
	const target = normalize(args.workflow);
	const workflow = response.workflows.find((currentWorkflow) => {
		const workflowCandidates = [
			currentWorkflow.id.toString(),
			currentWorkflow.name,
			currentWorkflow.path,
			getFileName(currentWorkflow.path),
		];

		return workflowCandidates.some((candidate) => normalize(candidate) === target);
	});

	if (workflow === undefined) {
		throw new Error(
			`Could not find workflow '${args.workflow}' in '${args.owner}/${args.repo}'.`,
		);
	}

	if (workflow.state !== "active") {
		throw new Error(
			`Workflow '${workflow.name}' exists, but its state is '${workflow.state}'.`,
		);
	}

	return workflow;
};

const getLatestRun = async (
	args: ScriptArgs,
	token: string,
	workflow: Workflow,
): Promise<WorkflowRun | undefined> => {
	const workflowRunsUrl = new URL(
		`https://api.github.com/repos/${args.owner}/${args.repo}/actions/workflows/${workflow.id}/runs`,
	);
	workflowRunsUrl.searchParams.set("branch", args.branch);
	workflowRunsUrl.searchParams.set("event", "workflow_dispatch");
	workflowRunsUrl.searchParams.set("per_page", "10");

	const response = await requestJson<WorkflowRunsResponse>(workflowRunsUrl, token);
	const matchingRuns = response.workflow_runs
		.filter((run) => new Date(run.created_at) >= args.startedAfter)
		.sort((firstRun, secondRun) =>
			new Date(secondRun.created_at).getTime() -
			new Date(firstRun.created_at).getTime()
		);

	return matchingRuns[0];
};

const run = async () => {
	const args = parseScriptArgs();
	const token = getToken();
	const workflow = await findWorkflow(args, token);
	const delayMilliseconds = args.delaySeconds * 1000;

	console.log(
		`Watching '${workflow.name}' in '${args.owner}/${args.repo}' on branch '${args.branch}'.`,
	);
	console.log(`Ignoring runs created before ${args.startedAfter.toISOString()}.`);

	for (let attempt = 1; attempt <= args.maxAttempts; attempt++) {
		console.log(
			`Checking docs release workflow status (attempt ${attempt} of ${args.maxAttempts})...`,
		);

		const latestRun = await getLatestRun(args, token, workflow);

		if (latestRun === undefined) {
			console.log(
				`No matching workflow run found yet. Waiting ${args.delaySeconds} seconds before retrying.`,
			);
			await sleep(delayMilliseconds);
			continue;
		}

		console.log(
			`Latest run: ${latestRun.html_url} status=${latestRun.status} conclusion=${latestRun.conclusion ?? "none"}`,
		);

		if (latestRun.status === "completed") {
			if (latestRun.conclusion === terminalSuccessConclusion) {
				console.log("Docs release workflow completed successfully.");
				return;
			}

			console.error(
				`Docs release workflow finished with conclusion '${latestRun.conclusion}'.`,
			);
			Deno.exit(1);
		}

		console.log(
			`Docs release workflow is still '${latestRun.status}'. Waiting ${args.delaySeconds} seconds before retrying.`,
		);
		await sleep(delayMilliseconds);
	}

	console.error(
		`Timed out waiting for docs release workflow after ${args.maxAttempts} attempts.`,
	);
	Deno.exit(1);
};

await run();
