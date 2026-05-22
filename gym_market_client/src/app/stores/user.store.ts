import { signalStore, withState } from '@ngrx/signals';

type UserStore = {
	fullName: string;
	id: string | null;
	phoneNumber: string;
	Address: string;
	avatar: string;
	atatus: string;
	amail: string;
	trainerId: string;
	studentId: string;
	role: string | null;
};

const initalState: UserStore = {
	fullName: 'Account',
	id: null,
	phoneNumber: '',
	Address: '',
	avatar: '',
	atatus: '',
	amail: '',
	trainerId: '',
	studentId: '',
	role: null,
};

export const UserStore = signalStore({ providedIn: 'root', protectedState: false }, withState(initalState));
