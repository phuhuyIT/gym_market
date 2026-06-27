export interface AdminUser {
	id: string;
	fullName?: string | null;
	email?: string | null;
	phoneNumber?: string | null;
	status: string;
	emailConfirmed: boolean;
	isLockedOut: boolean;
	lockoutEnd?: string | null;
	roles: string[];
	studentId?: string | null;
	trainerId?: string | null;
	trainerApprovalStatus?: string | null;
	createdAt?: string | null;
}

export interface AdminUserDetail extends AdminUser {
	address?: string | null;
	avatar?: string | null;
	accessFailedCount: number;
	twoFactorEnabled: boolean;
}
