export type CourseLiveSessionStatus = 'Draft' | 'Scheduled' | 'Completed' | 'Cancelled';

export interface CourseLiveSession {
	liveSessionId: string;
	courseId: string;
	title: string;
	description?: string | null;
	startsAt: string;
	endsAt: string;
	meetingUrl?: string | null;
	recordingUrl?: string | null;
	status: CourseLiveSessionStatus | string;
	attendanceRequired: boolean;
	createdAt: string;
	updatedAt: string;
	publishedAt?: string | null;
}

export interface UpsertCourseLiveSession {
	title: string;
	description?: string | null;
	startsAt: string | Date;
	endsAt: string | Date;
	meetingUrl?: string | null;
	recordingUrl?: string | null;
	status: CourseLiveSessionStatus | string;
	attendanceRequired: boolean;
}

export interface CourseCalendarItem {
	itemId: string;
	courseId: string;
	type: 'assignment' | 'quiz' | 'announcement' | 'live_session' | string;
	title: string;
	description?: string | null;
	startsAt: string;
	endsAt?: string | null;
	status?: string | null;
	link?: string | null;
}
