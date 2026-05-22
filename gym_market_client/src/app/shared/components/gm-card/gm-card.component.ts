import { Component } from '@angular/core';

@Component({
  selector: 'gm-card',
  template: `<div class="gm-card"><ng-content></ng-content></div>`,
  styleUrl: './gm-card.component.scss'
})
export class GmCardComponent {}
