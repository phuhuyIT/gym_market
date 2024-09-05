import { signalStore, withState } from '@ngrx/signals';

type UserStore = {
	fullName: string,
    id: string | null,
	phoneNumber: string
};

const initalState: UserStore = {
	fullName: 'Account',
	id: null,
	phoneNumber: ''
};

export const UserStore = signalStore({ providedIn: 'root' }, withState(initalState));
