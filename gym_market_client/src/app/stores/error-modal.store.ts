import { signalStore, withState } from '@ngrx/signals';

type ErrorState = {
	errors: string[];
	isShow: boolean;
};

const initialState: ErrorState = {
	errors: [],
	isShow: false,
};

export const ErrorModalStore = signalStore({ providedIn: 'root', protectedState: false }, withState(initialState));
