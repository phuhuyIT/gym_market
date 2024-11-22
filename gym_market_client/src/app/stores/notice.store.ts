import { signalStore, withState } from '@ngrx/signals';

type NoticeState = {
    message: string;
	isShow: boolean;
};

const initalState: NoticeState = {
    message: '',
	isShow: false,
};

export const NoticeModalStore = signalStore({ providedIn: 'root' }, withState(initalState));
