export interface CourseAnnouncement {
	announcementId: string;
	courseId: string;
	createdByUserId?: string | null;
	createdByRole: string;
	createdByName: string;
	title: string;
	body: string;
	isPinned: boolean;
	isPublished: boolean;
	publishedAt?: string | null;
	createdAt: string;
	updatedAt: string;
}

export interface UpsertCourseAnnouncement {
	title: string;
	body: string;
	isPinned: boolean;
	isPublished: boolean;
}
