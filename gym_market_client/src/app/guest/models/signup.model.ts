export interface SignUp {
    fullName: string;
    email: string;
    password: string;
    confirmPassword: string;
    role: string;
    healthStatus?: string;
    certification?: string;
    category?: string;
    bio?: string;
    experience?: number;
}
