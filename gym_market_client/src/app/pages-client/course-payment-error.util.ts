import { HttpErrorResponse } from '@angular/common/http';

const ERROR_MESSAGES: Record<string, string> = {
	COURSE_NOT_FOUND: 'This course is no longer available.',
	COURSE_NOT_PUBLISHED: 'This course is not open for enrollment yet.',
	COURSE_FULL: 'This course is full. Please choose another course.',
	REGISTRATION_NOT_FOUND: 'Please enroll in this course before paying.',
	REGISTRATION_CANCELED: 'This payment was canceled. Restart payment before choosing Momo.',
	MOMO_NOT_CONFIGURED: 'Momo payment is not configured yet. Please use bank transfer.',
	MOMO_PROVIDER_UNAVAILABLE: 'Momo payment is temporarily unavailable. Please try again later.',
	NOT_A_STUDENT: 'Please sign in with a student account.',
};

export function coursePaymentErrorMessage(error: unknown, fallback: string): string {
	const code = extractErrorCode(error);
	return code ? ERROR_MESSAGES[code] ?? fallback : fallback;
}

function extractErrorCode(error: unknown): string | null {
	if (!(error instanceof HttpErrorResponse)) {
		return null;
	}

	const body = error.error;
	if (typeof body === 'string') {
		return body;
	}

	if (!body || typeof body !== 'object') {
		return null;
	}

	const record = body as Record<string, unknown>;
	const code = record['message'] ?? record['Message'] ?? record['error'] ?? record['Error'];
	return typeof code === 'string' ? code : null;
}
