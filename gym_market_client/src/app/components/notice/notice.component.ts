import { Component, inject , ChangeDetectionStrategy } from '@angular/core';
import { NoticeModalStore } from '../../stores/notice.store';
import { patchState } from '@ngrx/signals';

@Component({
    selector: 'app-notice',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [],
    templateUrl: './notice.component.html',
    styleUrl: './notice.component.scss'
})
export class NoticeComponent {
	noticeStore = inject(NoticeModalStore);

	onCloseModal() {
		patchState(this.noticeStore, { isShow: false, message: '' });
	}
}
