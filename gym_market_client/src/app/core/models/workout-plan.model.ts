export interface UpsertWorkoutExercise {
	weekNumber: number;
	dayNumber: number;
	order: number;
	name: string;
	sets: number;
	reps: string;
	restSeconds: number;
	notes?: string | null;
}

export interface UpsertWorkoutPlan {
	trainerId?: string | null;
	name: string;
	goal?: string | null;
	difficulty: string;
	durationWeeks: number;
	isActive: boolean;
	exercises: UpsertWorkoutExercise[];
}

export interface WorkoutExercise {
	exerciseId: string;
	weekNumber: number;
	dayNumber: number;
	order: number;
	name: string;
	sets: number;
	reps: string;
	restSeconds: number;
	notes?: string | null;
	isCompleted: boolean;
	completedAt?: string | null;
}

export interface WorkoutPlan {
	workoutPlanId: string;
	trainerId?: string | null;
	trainerName?: string | null;
	name: string;
	goal?: string | null;
	difficulty: string;
	durationWeeks: number;
	isActive: boolean;
	createdAt: string;
	updatedAt?: string | null;
	exercises: WorkoutExercise[];
}

export interface StudentWorkoutAssignment {
	assignmentId: string;
	workoutPlanId: string;
	planName: string;
	goal?: string | null;
	difficulty: string;
	durationWeeks: number;
	studentId: string;
	studentName?: string | null;
	studentEmail?: string | null;
	trainerId?: string | null;
	trainerName?: string | null;
	status: string;
	startsAt: string;
	endsAt?: string | null;
	createdAt: string;
	completedAt?: string | null;
	cancelledAt?: string | null;
	totalExercises: number;
	completedExercises: number;
	completionPercent: number;
	exercises: WorkoutExercise[];
}
