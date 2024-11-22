import { signalStore, withState } from '@ngrx/signals';

type LoaderState = {
	isShow: boolean;
};

const initalState: LoaderState = {
	isShow: false,
};

export const LoaderModalStore = signalStore({ providedIn: 'root' }, withState(initalState));
