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
};

export const UserStore = signalStore({ providedIn: 'root' }, withState(initalState));
