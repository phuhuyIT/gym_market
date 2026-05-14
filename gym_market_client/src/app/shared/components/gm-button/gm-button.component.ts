import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'gm-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button
      [class]="'gm-btn gm-btn--' + variant"
      [disabled]="disabled || loading"
      (click)="clicked.emit()">
      <span *ngIf="loading" class="gm-btn__spinner"></span>
      <ng-content></ng-content>
    </button>
  `,
  styleUrl: './gm-button.component.scss'
})
export class GmButtonComponent {
  @Input() variant: 'primary' | 'secondary' | 'ghost' = 'primary';
  @Input() disabled = false;
  @Input() loading = false;
  @Output() clicked = new EventEmitter<void>();
}
