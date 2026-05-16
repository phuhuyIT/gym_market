import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { TrainerService } from '../../page-agency/trainer.service';
import { RouterLink } from '@angular/router';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Trainer } from '../../core/models/trainer.model';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GmButtonComponent } from '../../shared/components/gm-button/gm-button.component';

@Component({
	selector: 'app-trainer-list',
	standalone: true,
	imports: [RouterLink, NgIf, NgFor, FormsModule, GmButtonComponent],
	templateUrl: './trainer-list.component.html',
	styleUrl: './trainer-list.component.scss',
})
export class TrainerListComponent implements OnInit {
	trainers: Trainer[] = [];
	searchString: string = '';
	loader = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);

	constructor(private trainerService: TrainerService) {}

	ngOnInit() {
		this.getAllTrainers();
	}

	private getAllTrainers() {
		patchState(this.loader, { isShow: true });
		this.trainerService
			.getTrainers()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					patchState(this.loader, { isShow: false });
					this.trainers = res;
				},
				error: () => {
					patchState(this.loader, { isShow: false });
				},
			});
	}

	onSubmit() {
		// Cleanup: removed debug log
	}
}
