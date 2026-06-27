export interface CourseCertificate {
	certificateId: string;
	courseId: string;
	courseTitle?: string | null;
	studentId: string;
	studentName?: string | null;
	verificationCode: string;
	issuedAt: string;
}

export interface CourseCompletionStatus {
	courseId: string;
	totalLectures: number;
	completedLectures: number;
	lecturesCompleted: boolean;
	quizRequired: boolean;
	quizPassed: boolean;
	bestQuizScorePercent?: number | null;
	isCompleted: boolean;
	certificate?: CourseCertificate | null;
}
