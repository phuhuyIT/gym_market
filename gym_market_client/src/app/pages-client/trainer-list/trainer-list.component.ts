import { Component, inject } from '@angular/core';
import { TrainerService } from '../../page-agency/trainer.service';
import { RouterLink } from '@angular/router';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';

@Component({
	selector: 'app-trainer-list',
	standalone: true,
	imports: [RouterLink],
	templateUrl: './trainer-list.component.html',
	styleUrl: './trainer-list.component.scss',
})
export class TrainerListComponent {
	trainers: any;
	loader = inject(LoaderModalStore);

	constructor(private trainerService: TrainerService) {}

	ngOnInit() {
		this.trainers = [];

		this.getAllTrainers();
	}

	private getAllTrainers() {
		patchState(this.loader, { isShow: true });
		this.trainerService.getTrainers().subscribe({
			next: (res: any) => {
				patchState(this.loader, { isShow: false });
				this.trainers = res;
			},
		});
	}

	onSubmit() {
		console.log(123);
	}
}
