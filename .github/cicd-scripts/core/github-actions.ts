/**
 * Prints the given {@link errorMsg} to the console as a GitHub error.
 * @param errorMsg The error message to print.
 */
export function printGitHubError(errorMsg: string): void {
	console.log(`::error::${errorMsg}`);
}

/**
 * Prints the given {@link errorMsg} to the console as a GitHub notice.
 * @param errorMsg The error message to print.
 */
export function printGitHubNotice(errorMsg: string): void {
	console.log(`::notice::${errorMsg}`);
}
