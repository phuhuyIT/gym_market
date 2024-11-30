export interface TrainerInfoDto {
    trainerId: string;
    name: string;
    email: string;
    password: string;
    certification: string;
    bio: string;
    experience: number;
    rating: number;
    profilePicture: string;
    createdAt: Date;
    updatedAt: Date;
    userId: string;
    courses: any;
    messages: string[];
}