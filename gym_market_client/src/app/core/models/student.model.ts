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
  studentId: string;
  name?: string;
  email?: string;
  password?: string;
  healthStatus?: string;
  profilePicture?: string;
  updatedAt?: string;
}
