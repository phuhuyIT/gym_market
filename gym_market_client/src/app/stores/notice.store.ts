import { signalStore, withState } from '@ngrx/signals';

type NoticeState = {
    message: string;
	isShow: boolean;
};

const initialState: NoticeState = {
    message: '',
	isShow: false,
};

export const NoticeModalStore = signalStore({ providedIn: 'root', protectedState: false }, withState(initialState));
