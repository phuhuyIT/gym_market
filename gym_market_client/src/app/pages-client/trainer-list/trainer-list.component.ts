import { Component } from '@angular/core';
import { TrainerService } from '../../page-agency/trainer.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-trainer-list',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './trainer-list.component.html',
  styleUrl: './trainer-list.component.scss'
})
export class TrainerListComponent {
    trainers: any;

	constructor(private trainerService: TrainerService) {}

	ngOnInit() {
		this.trainers = [];

		this.getAllTrainers();
	}

	private getAllTrainers() {
		this.trainerService.getTrainers().subscribe({
			next: (res: any) => {
				this.trainers = res;
			},
		});
	}

	onSubmit() {
		console.log(123);
	}
}
