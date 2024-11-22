import { Component, inject } from '@angular/core';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../stores/error-modal.store';

@Component({
  selector: 'app-error-modal',
  standalone: true,
  imports: [],
  templateUrl: './error-modal.component.html',
  styleUrl: './error-modal.component.scss'
})
export class ErrorModalComponent {
  errorStore = inject(ErrorModalStore)
  
  constructor() {
    this.errorStore.errors().push("Email đã tồn tại", "Số điện thoại không hợp lệ")
  }

  onCloseModal() {
    patchState(this.errorStore, { isShow: false })
  }
}
