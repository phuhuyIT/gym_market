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

export interface StudentProfile {
  studentId: string;
  userId: string;
  profilePicture: string;
  healthStatus: string;
  createdAt: string;
  updatedAt: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  address: string;
  avatar: string;
  status: string | null;
}

export interface StudentProfileResponse {
  success: boolean;
  statusCode: number;
  errors: string[];
  message: string;
  studentProfile: StudentProfile;
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
