export const DEFAULT_AVATAR_URL = 'https://cdn-icons-png.flaticon.com/512/236/236832.png';
// Matches the backend group default (Defaults.AvatarUrl) so previews look like the saved result.
export const DEFAULT_GROUP_AVATAR_URL = 'https://cdn-icons-png.flaticon.com/512/1999/1999625.png';
export const DEFAULT_IMAGE_URL = '/assets/fitness_logo.jpg';
export const DEFAULT_COURSE_THUMBNAIL_URL = '/assets/gym_logo.png';
export const DEFAULT_AVATAR_IMAGE_URL = '/assets/fitness_logo.jpg';
export const DEFAULT_GROUP_AVATAR_IMAGE_URL = '/assets/icons/group.png';

export const TOAST_DURATION_MS = 3500;
export const SEARCH_DEBOUNCE_MS = 500;
export const SLIDE_INTERVAL_MS = 3000;

export function formatDateToInput(date: Date): string {
	const year = date.getFullYear();
	const month = (date.getMonth() + 1).toString().padStart(2, '0');
	const day = date.getDate().toString().padStart(2, '0');
	return `${year}-${month}-${day}`;
}
