export type AssignmentStatus = 'Draft' | 'Published' | 'Closed';
export type AssignmentSubmissionType = 'Text' | 'Url' | 'File';
export type AssignmentSubmissionStatus = 'Submitted' | 'Graded' | 'Returned';

export interface AssignmentSubmission {
	submissionId: string;
	assignmentId: string;
	studentId: string;
	studentName?: string | null;
	studentEmail?: string | null;
	textResponse?: string | null;
	attachmentUrl?: string | null;
	score?: number | null;
	scorePercent?: number | null;
	status: AssignmentSubmissionStatus | string;
	feedback?: string | null;
	submittedAt: string;
	gradedAt?: string | null;
	updatedAt: string;
}

export interface CourseAssignment {
	assignmentId: string;
	courseId: string;
	gradeCategoryId?: string | null;
	gradeCategoryName?: string | null;
	title: string;
	instructions?: string | null;
	pointsPossible: number;
	dueAt?: string | null;
	submissionType: AssignmentSubmissionType | string;
	status: AssignmentStatus | string;
	submissionCount: number;
	gradedCount: number;
	mySubmission?: AssignmentSubmission | null;
}

export interface UpsertCourseAssignment {
	title: string;
	instructions?: string | null;
	gradeCategoryId?: string | null;
	pointsPossible: number;
	dueAt?: string | null;
	submissionType?: AssignmentSubmissionType | string | null;
	status?: AssignmentStatus | string | null;
}

export interface SubmitAssignment {
	textResponse?: string | null;
	attachmentUrl?: string | null;
}
