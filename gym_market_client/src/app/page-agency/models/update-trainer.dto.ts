export interface UpdateTrainerProfileDto {
    trainerId: string;
	name: string;
	email: string;
	certification: string;
	bio: string;
	experience: number;
	rating: number;
	profilePicture: string;
	updatedAt: Date;
}
