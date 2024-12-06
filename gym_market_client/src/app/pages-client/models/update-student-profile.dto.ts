export interface UpdateStudentProfileDto {
	studentId: string;
	name: string;
	email: string;
	profilePicture: string;
	updatedAt: Date;
	userId: string | null;
	password: string;
}
