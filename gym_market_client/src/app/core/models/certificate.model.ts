export interface CourseCertificate {
	certificateId: string;
	courseId: string;
	courseTitle?: string | null;
	studentId: string;
	studentName?: string | null;
	verificationCode: string;
	issuedAt: string;
	setting?: CourseCertificateSetting | null;
}

export interface CourseCertificateSetting {
	courseId: string;
	courseTitle?: string | null;
	isEnabled: boolean;
	templateName: string;
	certificateTitle: string;
	bodyText: string;
	accentColor: string;
	requiredLecturePercent: number;
	requirePublishedQuizzes: boolean;
	requirePublishedAssignments: boolean;
	requiredAssignmentPercent: number;
	minimumFinalGradePercent?: number | null;
	updatedAt: string;
}

export interface UpdateCourseCertificateSetting {
	isEnabled: boolean;
	templateName: string;
	certificateTitle: string;
	bodyText: string;
	accentColor: string;
	requiredLecturePercent: number;
	requirePublishedQuizzes: boolean;
	requirePublishedAssignments: boolean;
	requiredAssignmentPercent: number;
	minimumFinalGradePercent?: number | null;
}

export interface CourseCompletionStatus {
	courseId: string;
	totalLectures: number;
	completedLectures: number;
	lecturesCompleted: boolean;
	lectureCompletionPercent: number;
	requiredLecturePercent: number;
	quizRequired: boolean;
	quizPassed: boolean;
	bestQuizScorePercent?: number | null;
	assignmentRequired: boolean;
	assignmentPassed: boolean;
	totalAssignments: number;
	gradedAssignments: number;
	assignmentAveragePercent?: number | null;
	requiredAssignmentPercent: number;
	finalGradePercent?: number | null;
	minimumFinalGradePercent?: number | null;
	finalGradePassed: boolean;
	certificatesEnabled: boolean;
	setting?: CourseCertificateSetting | null;
	isCompleted: boolean;
	certificate?: CourseCertificate | null;
}
