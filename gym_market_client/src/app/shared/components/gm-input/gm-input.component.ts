import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'gm-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="gm-input" [class.focused]="focused">
      <label class="gm-input__label" [class.active]="focused || value">{{ label }}</label>
      <input
        class="gm-input__field"
        [type]="type"
        [(ngModel)]="value"
        (ngModelChange)="valueChange.emit($event)"
        (focus)="focused = true"
        (blur)="focused = false"
        [placeholder]="focused ? '' : ''">
    </div>
  `,
  styleUrl: './gm-input.component.scss'
})
export class GmInputComponent {
  @Input() label = '';
  @Input() type = 'text';
  @Input() value = '';
  @Output() valueChange = new EventEmitter<string>();
  focused = false;
}
