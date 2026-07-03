export interface CourseAnalyticsDashboard {
	courseId: string;
	courseTitle?: string | null;
	totalLearners: number;
	totalLectures: number;
	totalAssignments: number;
	totalGradeItems: number;
	averageLessonProgressPercent: number;
	averageCurrentGradePercent?: number | null;
	averageFinalGradePercent: number;
	completedLearners: number;
	atRiskLearners: number;
	learners: CourseLearnerAnalytics[];
}

export interface MyCourseAnalytics {
	courseId: string;
	courseTitle?: string | null;
	progress: CourseLearnerAnalytics;
}

export interface CourseLearnerAnalytics {
	studentId: string;
	studentName?: string | null;
	studentEmail?: string | null;
	totalLectures: number;
	completedLectures: number;
	lessonProgressPercent: number;
	totalAssignments: number;
	submittedAssignments: number;
	gradedAssignments: number;
	missingAssignments: number;
	totalGradeItems: number;
	completedGradeItems: number;
	currentGradePercent?: number | null;
	finalGradePercent: number;
	letterGrade: string;
	lastActivityAt?: string | null;
	isCompleted: boolean;
	isAtRisk: boolean;
	atRiskReasons: string[];
}
