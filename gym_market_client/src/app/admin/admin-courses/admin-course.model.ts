export type CourseModerationStatus = 'Draft' | 'PendingReview' | 'Published' | 'Rejected' | 'Archived';

export interface AdminCourse {
	courseId: string;
	title?: string | null;
	description?: string | null;
	type?: string | null;
	category?: string | null;
	price?: number | null;
	startDate?: string | null;
	endDate?: string | null;
	status: CourseModerationStatus;
	trainerId?: string | null;
	trainerName?: string | null;
	trainerEmail?: string | null;
	trainerApprovalStatus?: string | null;
}
