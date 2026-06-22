export interface Trainer {
  trainerId: string;
  userId: string;
  name: string;
  email: string;
  profilePicture: string;
  bio: string;
  certification: string;
  category?: string;
  experience: number;
  rating: number;
  createdAt: string;
  updatedAt: string;
  bankBin?: string;
  bankAccountNo?: string;
  bankAccountName?: string;
}

export interface TrainerSearch {
  trainerId: string;
  userId?: string;
  name?: string;
  email?: string;
  fullName?: string;
  certification?: string;
  category?: string;
  bio?: string;
  experience?: number;
  rating?: number;
  profilePicture?: string;
  status?: string;
  createdAt?: string;
}

export interface UpdateTrainerProfileDto {
  trainerId: string;
  name: string;
  email: string;
  certification: string;
  category?: string;
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
