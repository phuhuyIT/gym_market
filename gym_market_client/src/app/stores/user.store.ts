import { signalStore, withState } from '@ngrx/signals';

type UserStore = {
	fullName: string;
	id: string | null;
	phoneNumber: string;
	address: string;
	avatar: string;
	status: string;
	email: string;
	trainerId: string;
	studentId: string;
	role: string | null;
};

const initialState: UserStore = {
	fullName: 'Account',
	id: null,
	phoneNumber: '',
	address: '',
	avatar: '',
	status: '',
	email: '',
	trainerId: '',
	studentId: '',
	role: null,
};

export const UserStore = signalStore({ providedIn: 'root', protectedState: false }, withState(initialState));
