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
	similarityScorePercent?: number | null;
	similarityMatchedSubmissionId?: string | null;
	similarityMatchedStudentName?: string | null;
	similarityFlags?: string | null;
	similarityCheckedAt?: string | null;
	submittedAt: string;
	gradedAt?: string | null;
	updatedAt: string;
	rubricScores: AssignmentRubricScore[];
	feedbackEntries: AssignmentFeedbackEntry[];
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
	rubricCriteria: AssignmentRubricCriterion[];
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
	rubricCriteria: UpsertAssignmentRubricCriterion[];
}

export interface SubmitAssignment {
	textResponse?: string | null;
	attachmentUrl?: string | null;
}

export interface ReturnAssignmentSubmission {
	feedback?: string | null;
}

export interface AssignmentRubricCriterion {
	criterionId: string;
	assignmentId: string;
	title: string;
	description?: string | null;
	pointsPossible: number;
	order: number;
}

export interface UpsertAssignmentRubricCriterion {
	criterionId?: string | null;
	title: string;
	description?: string | null;
	pointsPossible: number;
	order: number;
}

export interface GradeAssignmentRubricScore {
	criterionId: string;
	score: number;
	feedback?: string | null;
}

export interface AssignmentRubricScore {
	rubricScoreId: string;
	submissionId: string;
	criterionId: string;
	criterionTitle?: string | null;
	pointsPossible: number;
	score: number;
	feedback?: string | null;
}

export interface AssignmentFeedbackEntry {
	feedbackEntryId: string;
	submissionId: string;
	authorUserId?: string | null;
	authorName: string;
	authorRole: string;
	action: string;
	status: string;
	score?: number | null;
	scorePercent?: number | null;
	feedback?: string | null;
	createdAt: string;
}
