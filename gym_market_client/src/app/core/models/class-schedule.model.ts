export interface UpsertClassSession {
	title: string;
	description?: string | null;
	trainerId?: string | null;
	startsAt: string | Date;
	endsAt: string | Date;
	capacity: number;
	location?: string | null;
	status: string;
}

export interface ClassSession {
	classSessionId: string;
	title: string;
	description?: string | null;
	trainerId?: string | null;
	trainerName?: string | null;
	startsAt: string;
	endsAt: string;
	capacity: number;
	bookedCount: number;
	availableSpots: number;
	location?: string | null;
	status: string;
	isBooked: boolean;
	myBookingId?: string | null;
	myBookingStatus?: string | null;
}

export interface ClassBooking {
	bookingId: string;
	classSessionId: string;
	classTitle: string;
	studentId: string;
	studentName?: string | null;
	studentEmail?: string | null;
	status: string;
	bookedAt: string;
	cancelledAt?: string | null;
	attendanceMarkedAt?: string | null;
}
