import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TrainerService } from '../trainer.service';
import { UserInfoDto } from '../../user/models/user-info.dto';
import { UserStore } from '../../stores/user.store';
import { UserService } from '../../user/user.service';
import { TrainerInfoDto } from '../models/trainer-inf0.dto';

@Component({
	selector: 'app-your-profile',
	standalone: true,
	imports: [RouterLink],
	templateUrl: './your-profile.component.html',
	styleUrl: './your-profile.component.scss',
})
export class YourProfileComponent {
	userInfo!: UserInfoDto;
	userStore = inject(UserStore);
	trainerInfo!: TrainerInfoDto;

	constructor(
		private trainerService: TrainerService,
		private router: Router,
		private userService: UserService
	) {}

	ngOnInit() {
		this.userInfo = {
			address: 'https://cdn-icons-png.flaticon.com/512/236/236832.png',
			avatar: '',
			email: '',
			fullName: '',
			id: '',
			phoneNumber: '',
			status: '',
		};

		this.trainerInfo = {
			bio: '',
			certification: '',
			courses: [],
			createdAt: new Date(),
			email: '',
			experience: 0,
			messages: [],
			name: '',
			password: '',
			profilePicture: '',
			rating: 0,
			trainerId: '',
			updatedAt: new Date(),
			userId: '',
		};

		this.getUserInfo();
		this.getTrainerInfo();
	}

	private getUserInfo() {
		if (this.userStore.id() !== null) {
			this.userService.getUserInfo(this.userStore.id()).subscribe({
				next: (res: any) => {
					this.userInfo = { ...res.userInfo };
					if (this.userInfo.avatar === null) {
						this.userInfo.avatar =
							'https://cdn-icons-png.flaticon.com/512/236/236832.png';
					}
				},
				error: error => {
					this.router.navigateByUrl('/login');
				},
			});
		}
	}

	private getTrainerInfo() {
		this.trainerService.getTrainerInfo(this.userStore.trainerId()).subscribe({
			next: (res: any) => {
				this.trainerInfo = { ...res };
                if (this.trainerInfo.profilePicture === null) {
                    this.trainerInfo.profilePicture =
                        'https://cdn-icons-png.flaticon.com/512/236/236832.png';
                }
			},
			error: error => {
				this.router.navigateByUrl('/login');
			},
		});
	}
}
