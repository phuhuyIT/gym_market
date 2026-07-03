export type DiscussionQuestionStatus = 'Open' | 'Answered' | 'Closed';
export type DiscussionAuthorRole = 'Student' | 'Trainer' | 'Admin';

export interface DiscussionAnswer {
	answerId: string;
	questionId: string;
	authorUserId?: string | null;
	authorEntityId?: string | null;
	authorRole: DiscussionAuthorRole | string;
	authorName: string;
	authorEmail?: string | null;
	body: string;
	isAccepted: boolean;
	canDelete: boolean;
	createdAt: string;
	updatedAt: string;
}

export interface DiscussionQuestion {
	questionId: string;
	courseId: string;
	studentId: string;
	studentName?: string | null;
	studentEmail?: string | null;
	title: string;
	body: string;
	status: DiscussionQuestionStatus | string;
	isPinned: boolean;
	acceptedAnswerId?: string | null;
	answerCount: number;
	canAccept: boolean;
	createdAt: string;
	updatedAt: string;
	lastActivityAt: string;
	answers: DiscussionAnswer[];
}

export interface CreateDiscussionQuestion {
	title: string;
	body: string;
}

export interface CreateDiscussionAnswer {
	body: string;
}

export interface ModerateDiscussionQuestion {
	status?: DiscussionQuestionStatus | string | null;
	isPinned?: boolean | null;
}
