export type AssessmentScopeType = 'Course' | 'Module' | 'Lesson';
export type QuizQuestionType = 'SingleChoice' | 'MultipleChoice' | 'OpenText';

export interface QuizAttemptSummary {
	attemptId: string;
	quizId: string;
	studentId: string;
	studentName?: string | null;
	studentEmail?: string | null;
	attemptNumber: number;
	score: number;
	totalPoints: number;
	scorePercent: number;
	passed: boolean;
	status: string;
	requiresManualGrading: boolean;
	startedAt?: string | null;
	honorCodeAccepted: boolean;
	proctoringReviewRequired: boolean;
	focusLostCount: number;
	pasteEventCount: number;
	fullscreenExitCount: number;
	proctoringEventCount: number;
	suspiciousActivityScore: number;
	proctoringFlags?: string | null;
	feedback?: string | null;
	submittedAt: string;
}

export interface CourseQuiz {
	quizId: string;
	courseId: string;
	title: string;
	description?: string | null;
	scopeType: AssessmentScopeType | string;
	moduleId?: string | null;
	moduleTitle?: string | null;
	lectureId?: string | null;
	lectureTitle?: string | null;
	passingScorePercent: number;
	timeLimitMinutes?: number | null;
	maxAttempts?: number | null;
	attemptsUsed: number;
	attemptsRemaining?: number | null;
	shuffleQuestions: boolean;
	shuffleOptions: boolean;
	showCorrectAnswers: boolean;
	requireHonorCode: boolean;
	trackProctoringSignals: boolean;
	availableFrom?: string | null;
	availableUntil?: string | null;
	isPublished: boolean;
	questions: QuizQuestion[];
	latestAttempt?: QuizAttemptSummary | null;
	bestAttempt?: QuizAttemptSummary | null;
}

export interface QuizQuestion {
	questionId: string;
	prompt: string;
	questionType: QuizQuestionType | string;
	order: number;
	points: number;
	explanation?: string | null;
	questionBank?: string | null;
	options: QuizOption[];
}

export interface QuizOption {
	optionId: string;
	text: string;
}

export interface TrainerCourseQuiz {
	quizId: string;
	courseId: string;
	title: string;
	description?: string | null;
	scopeType: AssessmentScopeType | string;
	moduleId?: string | null;
	moduleTitle?: string | null;
	lectureId?: string | null;
	lectureTitle?: string | null;
	passingScorePercent: number;
	timeLimitMinutes?: number | null;
	maxAttempts?: number | null;
	shuffleQuestions: boolean;
	shuffleOptions: boolean;
	showCorrectAnswers: boolean;
	requireHonorCode: boolean;
	trackProctoringSignals: boolean;
	availableFrom?: string | null;
	availableUntil?: string | null;
	isPublished: boolean;
	questions: TrainerQuizQuestion[];
}

export interface TrainerQuizQuestion {
	questionId?: string;
	prompt: string;
	questionType: QuizQuestionType | string;
	order: number;
	points: number;
	explanation?: string | null;
	questionBank?: string | null;
	requiresManualGrading?: boolean;
	options: TrainerQuizOption[];
}

export interface TrainerQuizOption {
	optionId?: string;
	text: string;
	isCorrect: boolean;
}

export interface UpsertCourseQuiz {
	title: string;
	description?: string | null;
	scopeType: AssessmentScopeType | string;
	moduleId?: string | null;
	lectureId?: string | null;
	passingScorePercent: number;
	timeLimitMinutes?: number | null;
	maxAttempts?: number | null;
	shuffleQuestions: boolean;
	shuffleOptions: boolean;
	showCorrectAnswers: boolean;
	requireHonorCode: boolean;
	trackProctoringSignals: boolean;
	availableFrom?: string | null;
	availableUntil?: string | null;
	isPublished: boolean;
	questions: TrainerQuizQuestion[];
}

export interface SubmitQuizAttempt {
	startedAt?: string | null;
	honorCodeAccepted: boolean;
	browserFingerprint?: string | null;
	proctoringSignals: QuizProctoringSignal[];
	answers: {
		questionId: string;
		selectedOptionId?: string | null;
		selectedOptionIds?: string[];
		textAnswer?: string | null;
	}[];
}

export interface QuizProctoringSignal {
	type: string;
	occurredAt?: string | null;
	count: number;
}
