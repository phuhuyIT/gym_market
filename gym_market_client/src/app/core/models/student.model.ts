export interface Student {
  studentId: string;
  userId: string;
  name: string;
  email: string;
  profilePicture: string;
  healthStatus: string;
  createdAt: string;
  updatedAt: string;
}

export interface UpdateStudentProfileDto {
  fullName: string;
  height: number;
  weight: number;
  dateOfBirth: string;
  avatar?: string;
}
