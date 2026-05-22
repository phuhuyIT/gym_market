import { Component, inject } from '@angular/core';
import { LoaderModalStore } from '../../stores/loader.store';

@Component({
    selector: 'app-loader',
    imports: [],
    templateUrl: './loader.component.html',
    styleUrl: './loader.component.scss'
})
export class LoaderComponent {
	loaderStore = inject(LoaderModalStore);
}
