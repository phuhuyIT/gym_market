export interface QuizAttemptSummary {
	attemptId: string;
	quizId: string;
	studentId: string;
	studentName?: string | null;
	studentEmail?: string | null;
	score: number;
	totalPoints: number;
	scorePercent: number;
	passed: boolean;
	submittedAt: string;
}

export interface CourseQuiz {
	quizId: string;
	courseId: string;
	title: string;
	passingScorePercent: number;
	isPublished: boolean;
	questions: QuizQuestion[];
	latestAttempt?: QuizAttemptSummary | null;
	bestAttempt?: QuizAttemptSummary | null;
}

export interface QuizQuestion {
	questionId: string;
	prompt: string;
	order: number;
	points: number;
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
	passingScorePercent: number;
	isPublished: boolean;
	questions: TrainerQuizQuestion[];
}

export interface TrainerQuizQuestion {
	questionId?: string;
	prompt: string;
	order: number;
	points: number;
	options: TrainerQuizOption[];
}

export interface TrainerQuizOption {
	optionId?: string;
	text: string;
	isCorrect: boolean;
}

export interface UpsertCourseQuiz {
	title: string;
	passingScorePercent: number;
	isPublished: boolean;
	questions: TrainerQuizQuestion[];
}

export interface SubmitQuizAttempt {
	answers: {
		questionId: string;
		selectedOptionId?: string | null;
	}[];
}
