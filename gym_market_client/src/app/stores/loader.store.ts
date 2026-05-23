import { signalStore, withState } from '@ngrx/signals';

type LoaderState = {
	isShow: boolean;
};

const initialState: LoaderState = {
	isShow: false,
};

export const LoaderModalStore = signalStore({ providedIn: 'root', protectedState: false }, withState(initialState));
