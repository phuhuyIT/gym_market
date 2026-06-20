export interface Trainer {
  trainerId: string;
  userId: string;
  name: string;
  email: string;
  profilePicture: string;
  bio: string;
  certification: string;
  experience: number;
  rating: number;
  createdAt: string;
  updatedAt: string;
  bankBin?: string;
  bankAccountNo?: string;
  bankAccountName?: string;
}

export interface UpdateTrainerProfileDto {
  trainerId: string;
  name: string;
  email: string;
  certification: string;
  bio: string;
  experience: number;
  rating: number;
  profilePicture: string;
  updatedAt: string | Date;
  userId: string;
  bankBin?: string;
  bankAccountNo?: string;
  bankAccountName?: string;
}
