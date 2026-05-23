import { Component, inject , ChangeDetectionStrategy } from '@angular/core';
import { LoaderModalStore } from '../../stores/loader.store';

@Component({
    selector: 'app-loader',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [],
    templateUrl: './loader.component.html',
    styleUrl: './loader.component.scss'
})
export class LoaderComponent {
	loaderStore = inject(LoaderModalStore);
}
