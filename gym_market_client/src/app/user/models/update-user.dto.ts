export interface UpdateUserDto {
	id: string | null;
	fullName: string;
	address: string;
	avatar: string;
	status: string | null;
	phoneNumber: string;
}
