export interface UpsertProgressLog {
	loggedAt?: string | Date | null;
	weightKg?: number | null;
	bodyFatPercent?: number | null;
	waistCm?: number | null;
	chestCm?: number | null;
	armCm?: number | null;
	hipCm?: number | null;
	strengthNotes?: string | null;
	notes?: string | null;
}

export interface ProgressLog {
	progressLogId: string;
	studentId: string;
	studentName?: string | null;
	studentEmail?: string | null;
	loggedAt: string;
	weightKg?: number | null;
	bodyFatPercent?: number | null;
	waistCm?: number | null;
	chestCm?: number | null;
	armCm?: number | null;
	hipCm?: number | null;
	strengthNotes?: string | null;
	notes?: string | null;
}

export interface UpsertProgressGoal {
	targetWeightKg?: number | null;
	targetBodyFatPercent?: number | null;
	goalDate?: string | Date | null;
	status: string;
	notes?: string | null;
}

export interface ProgressGoal {
	progressGoalId: string;
	studentId: string;
	targetWeightKg?: number | null;
	targetBodyFatPercent?: number | null;
	goalDate?: string | null;
	status: string;
	notes?: string | null;
	createdAt: string;
	updatedAt?: string | null;
}

export interface ProgressSummary {
	studentId: string;
	studentName?: string | null;
	studentEmail?: string | null;
	logCount: number;
	latestLoggedAt?: string | null;
	latestWeightKg?: number | null;
	weightChangeKg?: number | null;
	latestBodyFatPercent?: number | null;
	bodyFatChangePercent?: number | null;
	activeGoal?: ProgressGoal | null;
	goalStatusLabel: string;
}
