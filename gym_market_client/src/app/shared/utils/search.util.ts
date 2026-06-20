export function normalizeSearch(value: string | null | undefined): string {
	return (value ?? '').trim().toLowerCase();
}

export function matchesSearch(query: string | null | undefined, values: Array<string | null | undefined>): boolean {
	const normalizedQuery = normalizeSearch(query);
	if (!normalizedQuery) return true;

	return values.some(value => normalizeSearch(value).includes(normalizedQuery));
}
