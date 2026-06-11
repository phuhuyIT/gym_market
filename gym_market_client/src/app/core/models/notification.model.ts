export interface AppNotification {
	id: number;
	type: string;
	title: string;
	content: string | null;
	link: string | null;
	isRead: boolean;
	createdAt: string;
}
