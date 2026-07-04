export interface CourseStudyGroup {
	studyGroupId: string;
	courseId: string;
	conversationId: number;
	name: string;
	description?: string | null;
	kind: 'Cohort' | 'StudyGroup' | string;
	isDefaultCohort: boolean;
	isActive: boolean;
	isMember: boolean;
	canManage: boolean;
	memberCount: number;
	createdAt: string;
	updatedAt: string;
	members: CourseStudyGroupMember[];
}

export interface CourseStudyGroupMember {
	userId: string;
	studentId?: string | null;
	fullName: string;
	email: string;
	avatar: string;
	role: 'Owner' | 'Admin' | 'Member' | string;
	joinedAt: string;
	isEligibleLearner: boolean;
}

export interface EligibleCourseLearner {
	studentId: string;
	userId: string;
	fullName: string;
	email: string;
	avatar: string;
	isInGroup: boolean;
}

export interface UpsertCourseStudyGroup {
	name: string;
	description?: string | null;
	kind?: 'Cohort' | 'StudyGroup' | string;
	isActive: boolean;
	memberUserIds: string[];
}
