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
	completionRatePercent: number;
	atRiskRatePercent: number;
	submissionRatePercent: number;
	averageEngagementScore: number;
	engagement: CourseEngagementSummary;
	performanceItems: CoursePerformanceItem[];
	trends: CourseAnalyticsTrend[];
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
	quizAttempts: number;
	discussionPosts: number;
	certificateIssued: boolean;
	upcomingItems: number;
	engagementScore: number;
	riskScore: number;
	recommendedAction: string;
	isCompleted: boolean;
	isAtRisk: boolean;
	atRiskReasons: string[];
}

export interface CourseEngagementSummary {
	discussionQuestions: number;
	discussionAnswers: number;
	activeStudyGroups: number;
	certificatesIssued: number;
	upcomingCalendarItems: number;
	scheduledLiveSessions: number;
}

export interface CoursePerformanceItem {
	itemId: string;
	title: string;
	itemType: string;
	averagePercent?: number | null;
	passRatePercent: number;
	completedCount: number;
	missingCount: number;
	totalLearners: number;
}

export interface CourseAnalyticsTrend {
	weekStart: string;
	completedLessons: number;
	assignmentSubmissions: number;
	quizAttempts: number;
	discussionPosts: number;
}
