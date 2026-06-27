export interface MembershipPlan {
	planId: string;
	name: string;
	description?: string | null;
	durationDays: number;
	price: number;
	isActive: boolean;
	createdAt: string;
	updatedAt?: string | null;
}

export interface UpsertMembershipPlan {
	name: string;
	description?: string | null;
	durationDays: number;
	price: number;
	isActive: boolean;
}

export interface StudentMembership {
	membershipId: string;
	planId: string;
	planName: string;
	studentId: string;
	studentName?: string | null;
	studentEmail?: string | null;
	status: 'Active' | 'Cancelled' | 'Expired' | string;
	startsAt: string;
	endsAt: string;
	cancelledAt?: string | null;
	price: number;
	durationDays: number;
}

export interface MembershipStatus {
	hasActiveMembership: boolean;
	currentMembership?: StudentMembership | null;
	availablePlans: MembershipPlan[];
}
