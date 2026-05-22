import { Component, input, output } from '@angular/core';


@Component({
    selector: 'gm-button',
    imports: [],
    template: `
    <button
      [class]="'gm-btn gm-btn--' + variant()"
      [disabled]="disabled() || loading()"
      (click)="clicked.emit()">
      @if (loading()) {
        <span class="gm-btn__spinner"></span>
      }
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
