import { Component , ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-ambient',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [],
    templateUrl: './ambient.component.html',
    styleUrl: './ambient.component.scss'
})
export class AmbientComponent {}
