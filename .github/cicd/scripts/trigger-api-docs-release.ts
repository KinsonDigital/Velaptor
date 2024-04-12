import { WorkflowClient } from "clients/WorkflowClient.ts";
import { Confirm } from "https://deno.land/x/cliffy@v1.0.0-rc.4/prompt/confirm.ts";

// Check that the required number of arguments were provided
if (Deno.args.length != 5) {
    let errorMsg = `The required number of arguments is 5 but only '${Deno.args.length}' was provided`;
    errorMsg += "\nThe required arguments are as follows:";
    errorMsg += "\n1. Repository Name: The name of the repository where the workflow is located.";
    errorMsg += "\n2. Branch Name: The name of the branch where the workflow is located.";
    errorMsg += "\n3. Workflow Name: The name of the workflow to execute.";
    errorMsg += "\n4. Workflow Inputs: The comma-delimited list of inputs to pass to the workflow.";
    errorMsg += "\n\tNOTE: Syntax is a comma-delimited list of key value pairs wrapped in square brackets.";
    errorMsg += "\n\tExample: '[item1,value1]|[item2, value1]'";
    errorMsg += "\n5. GitHub PAT: The GitHub Personal Access Token to use for authentication.";
    Deno.exit(1);
}

const doNotExecuteRelease = !(await Confirm.prompt({
    message: "Are you sure you want to trigger an API docs release?",
    validate: (value) => {
        return value.length === 1 && value[0].toUpperCase() === "Y" || value[0].toUpperCase() === "N";
    },
}));

if (doNotExecuteRelease) {
    console.log("API docs release cancelled.");
    Deno.exit();
}

console.log("Executing API docs release...");

const args = Deno.args.map((arg) => arg.trim());
const repoName = args[0].trim();
const branchName = args[1].trim();
const workflowFileName = args[2].trim();
const rawWorkflowInputList = args[3].trim();
const token = args[4].trim();

// First get a list of the workflow tuples(key value pairs)
const tuples: string[] = rawWorkflowInputList.indexOf("|") != -1
    ? rawWorkflowInputList.split("|").map((input) => input.trim().replace("[", "").replace("]", ""))
    : [rawWorkflowInputList.trim().replace("[", "").replace("]", "")];

const workflowInputs: [string, string][] = [];

for (const tuple of tuples) {
    const [key, value] = tuple.split(",").map((input) => input.trim());
    workflowInputs.push([key, value]);
}

const containsPAT = (value: string): boolean => {
    const fineGrainedTokenPrefix = "github_pat_";
    const classicTokenPrefix = "ghp_";

    return value.startsWith(fineGrainedTokenPrefix) || value.startsWith(classicTokenPrefix);
}

const printGitHubError = (errorMsg: string): void => {
    console.log(`::error::${errorMsg}`);
};

const toOrdinal = (number: number): string => {
    const suffixes = ["th", "st", "nd", "rd"];
    const value = Math.abs(number) % 100;
    const suffix = suffixes[(value - 20) % 10] || suffixes[value] || suffixes[0];

    return `${number}${suffix}`;
};

const argNames = ["Repository Name", "Branch Name", "Workflow Name", "Workflow Inputs", "GitHub PAT"];

// Print out all of the arguments as long as they don't contain a PAT
for (let i = 0; i < args.length; i++) {
    const arg = args[i];
    
    if (i === args.length - 1) {
        console.log(`${argNames[i]}: ***`);

        if (!containsPAT(token)) {
            const errorMsg = `The ${toOrdinal(i + 1)} argument (GitHub PAT) is not a valid GitHub personal access token.`;
            printGitHubError(errorMsg);
            Deno.exit(1);
        }
    } else {
        const isInvalid = containsPAT(arg);
        console.log(`${argNames[i]}: ${isInvalid ? "***" : args[i]}`);

        if (isInvalid) {
            const errorMsg = `A GitHub PAT was found in the ${toOrdinal(i + 1)} argument. This is not allowed.`;
            printGitHubError(errorMsg);
            
            Deno.exit(1);
        }
    }
}

const client = new WorkflowClient(token);
await client.executeWorkflow(repoName, branchName, workflowFileName, workflowInputs);
