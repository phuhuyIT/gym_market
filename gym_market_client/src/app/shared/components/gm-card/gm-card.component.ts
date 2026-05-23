import { Component , ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'gm-card',
    changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<div class="gm-card"><ng-content></ng-content></div>`,
  styleUrl: './gm-card.component.scss'
})
export class GmCardComponent {}
