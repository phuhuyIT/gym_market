/** File-upload limits and accepted types. Keep in sync with server-side validation. */
export const MAX_AVATAR_BYTES = 5 * 1024 * 1024; // 5 MB
export const MAX_VIDEO_BYTES = 100 * 1024 * 1024; // 100 MB

export const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/webp', 'image/gif'];
