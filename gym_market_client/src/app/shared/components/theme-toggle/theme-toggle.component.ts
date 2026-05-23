import { Component, inject , ChangeDetectionStrategy } from '@angular/core';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
    selector: 'app-theme-toggle',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [],
    templateUrl: './theme-toggle.component.html',
    styleUrl: './theme-toggle.component.scss'
})
export class ThemeToggleComponent {
  theme = inject(ThemeService);
}
