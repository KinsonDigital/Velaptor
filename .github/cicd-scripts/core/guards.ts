/**
 * Checks if the value is null, undefined, or empty.
 * @param value The value to check.
 * @returns True if the value is null, undefined, or empty, otherwise false.
 */
export function isNothing<T>(
	value:
		| undefined
		| null
		| string
		| number
		| boolean
		| T[]
		| (() => T)
		| object,
): value is undefined | null | "" | number | T[] | (() => T) | object {
	if (value === undefined || value === null) {
		return true;
	}

	if (typeof value === "string") {
		return value === "";
	}

	if (Array.isArray(value)) {
		return value.length === 0;
	}

	return false;
}
