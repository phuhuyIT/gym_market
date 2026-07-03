export interface GradeCategory {
	categoryId: string;
	name: string;
	weightPercent: number;
	order: number;
	isDefault: boolean;
}

export interface GradeItem {
	itemId: string;
	title: string;
	itemType: string;
	categoryId?: string | null;
	pointsPossible: number;
	isPublished: boolean;
}

export interface UpdateGradeCategory {
	categoryId?: string | null;
	name: string;
	weightPercent: number;
	order: number;
}

export interface UpdateGradeItemCategory {
	itemId: string;
	categoryId?: string | null;
}

export interface UpdateGradebookPolicy {
	categories: UpdateGradeCategory[];
	items: UpdateGradeItemCategory[];
}

export interface GradebookPolicy {
	courseId: string;
	courseTitle?: string | null;
	categories: GradeCategory[];
	items: GradeItem[];
}

export interface GradeItemScore {
	itemId: string;
	title: string;
	itemType: string;
	categoryId: string;
	pointsPossible: number;
	score?: number | null;
	totalPoints?: number | null;
	scorePercent?: number | null;
	status: string;
	submittedAt?: string | null;
}

export interface CategoryGrade {
	categoryId: string;
	name: string;
	weightPercent: number;
	currentPercent?: number | null;
	finalPercent: number;
	weightedPoints: number;
	gradedItems: number;
	totalItems: number;
}

export interface StudentGradeSummary {
	studentId: string;
	studentName?: string | null;
	studentEmail?: string | null;
	currentPercent?: number | null;
	finalPercent: number;
	letterGrade: string;
	categories: CategoryGrade[];
	items: GradeItemScore[];
}

export interface CourseGradebook {
	courseId: string;
	courseTitle?: string | null;
	categories: GradeCategory[];
	items: GradeItem[];
	students: StudentGradeSummary[];
	currentAveragePercent?: number | null;
	finalAveragePercent: number;
}

export interface MyCourseGrades {
	courseId: string;
	courseTitle?: string | null;
	categories: GradeCategory[];
	items: GradeItem[];
	grade: StudentGradeSummary;
}
