import { Component, input, model } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'gm-input',
    imports: [CommonModule, FormsModule],
    template: `
    <div class="gm-input-group">
      <label class="gm-input-label">{{ label() }}</label>
      <input
        class="gm-input-field"
        [type]="type()"
        [ngModel]="value()"
        (ngModelChange)="value.set($event)"
        [placeholder]="placeholder()">
    </div>
  `,
    styleUrl: './gm-input.component.scss'
})
export class GmInputComponent {
  label = input('');
  type = input('text');
  placeholder = input('');
  value = model<string | number>('');
}
