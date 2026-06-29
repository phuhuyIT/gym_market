export interface AdminNotificationTemplate {
	type: string;
	label: string;
	subjectTemplate: string;
	bodyTemplate: string;
	isActive: boolean;
	updatedAt: string | null;
	variables: string[];
}

export interface UpdateNotificationTemplate {
	subjectTemplate: string;
	bodyTemplate: string;
	isActive: boolean;
}

export interface NotificationDeliveryLog {
	id: number;
	userId: string;
	recipientEmail: string | null;
	recipientName: string | null;
	type: string;
	typeLabel: string;
	channel: string;
	status: string;
	subject: string;
	content: string | null;
	link: string | null;
	errorMessage: string | null;
	createdAt: string;
}
