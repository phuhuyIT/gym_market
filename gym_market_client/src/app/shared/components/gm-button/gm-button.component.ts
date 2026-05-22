import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'gm-button',
    imports: [CommonModule],
    template: `
    <button
      [class]="'gm-btn gm-btn--' + variant()"
      [disabled]="disabled() || loading()"
      (click)="clicked.emit()">
      <span *ngIf="loading()" class="gm-btn__spinner"></span>
      <ng-content></ng-content>
    </button>
  `,
    styleUrl: './gm-button.component.scss'
})
export class GmButtonComponent {
  variant = input<'primary' | 'secondary' | 'ghost'>('primary');
  disabled = input(false);
  loading = input(false);
  clicked = output<void>();
}
