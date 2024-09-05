import { signalStore, withState } from '@ngrx/signals';

type ErrorState = {
	errors: string[];
	isShow: boolean;
};

const initalState: ErrorState = {
	errors: [],
	isShow: false,
};

export const ErrorModalStore = signalStore({ providedIn: 'root' }, withState(initalState));
