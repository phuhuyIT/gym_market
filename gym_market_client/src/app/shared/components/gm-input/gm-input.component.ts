import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'gm-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="gm-input-group">
      <label class="gm-input-label">{{ label }}</label>
      <input
        class="gm-input-field"
        [type]="type"
        [(ngModel)]="value"
        (ngModelChange)="valueChange.emit($event)"
        [placeholder]="placeholder">
    </div>
  `,
  styleUrl: './gm-input.component.scss'
})
export class GmInputComponent {
  @Input() label = '';
  @Input() type = 'text';
  @Input() placeholder = '';
  @Input() value: string | number = '';
  @Output() valueChange = new EventEmitter<any>();
}
