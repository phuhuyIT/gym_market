import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../services/toast.service';

@Component({
    selector: 'gm-toast',
    imports: [CommonModule],
    template: `
    <div class="gm-toast-container">
      @for (toast of toasts$ | async; track toast) {
        <div
          class="gm-toast"
          [class]="'gm-toast--' + toast.type"
          (click)="dismiss(toast.id)">
          <span class="gm-toast__icon">
            @if (toast.type === 'success') {
              <i class="fas fa-check-circle"></i>
            }
            @if (toast.type === 'error') {
              <i class="fas fa-exclamation-circle"></i>
            }
          </span>
          <span class="gm-toast__message">{{ toast.message }}</span>
        </div>
      }
    </div>
    `,
    styleUrl: './gm-toast.component.scss'
})
export class GmToastComponent {
  private toastService = inject(ToastService);
  toasts$ = this.toastService.toasts$;

  dismiss(id: number) {
    this.toastService.dismiss(id);
  }
}
