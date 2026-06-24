import { HttpErrorResponse } from '@angular/common/http';

const paymentActionMessages: Record<string, string> = {
	PAYMENT_NOT_FOUND: 'Payment was not found. Refresh the list and try again.',
	PAYMENT_ALREADY_FINALIZED: 'Only pending payments can be changed.',
	PAYMENT_OBSOLETE: 'This payment attempt was replaced by another successful payment.',
	GATEWAY_PAYMENT_MANUAL_APPROVAL_NOT_ALLOWED: 'Momo payments are confirmed by Momo automatically.'
};

export function paymentActionErrorMessage(error: unknown, fallback: string): string {
	if (error instanceof HttpErrorResponse) {
		const code = error.error?.errorCode || error.error?.message;
		if (typeof code === 'string' && paymentActionMessages[code]) {
			return paymentActionMessages[code];
		}
		if (typeof error.error?.message === 'string') {
			return error.error.message;
		}
	}

	return fallback;
}
