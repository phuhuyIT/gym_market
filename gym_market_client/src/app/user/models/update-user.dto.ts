// The user being updated is derived from the JWT on the backend.
export interface UpdateUserDto {
	fullName: string;
	address: string;
	avatar: string;
	status: string | null;
	phoneNumber: string;
}
