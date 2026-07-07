import { Component, input, output , ChangeDetectionStrategy } from '@angular/core';


@Component({
    selector: 'gm-button',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [],
    template: `
    <button
      [class]="'gm-btn gm-btn--' + variant()"
      [disabled]="disabled() || loading()"
      [attr.aria-label]="ariaLabel() || null"
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
  ariaLabel = input<string | null>(null);
  clicked = output<void>();
}
