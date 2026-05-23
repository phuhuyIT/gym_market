export const DEFAULT_AVATAR_URL = 'https://cdn-icons-png.flaticon.com/512/236/236832.png';

export const TOAST_DURATION_MS = 3500;
export const SEARCH_DEBOUNCE_MS = 500;
export const SLIDE_INTERVAL_MS = 3000;

export function formatDateToInput(date: Date): string {
	const year = date.getFullYear();
	const month = (date.getMonth() + 1).toString().padStart(2, '0');
	const day = date.getDate().toString().padStart(2, '0');
	return `${year}-${month}-${day}`;
}
