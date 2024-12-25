import { Component, inject } from '@angular/core';
import { TrainerService } from '../../page-agency/trainer.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { TrainerInfoDto } from '../../page-agency/models/trainer-inf0.dto';
import { UserService } from '../../user/user.service';
import { UserInfoDto } from '../../user/models/user-info.dto';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { ConversationService } from '../../chat/conversation.service';
import { UserStore } from '../../stores/user.store';

@Component({
	selector: 'app-trainder-details',
	standalone: true,
	imports: [RouterLink],
	templateUrl: './trainder-details.component.html',
	styleUrl: './trainder-details.component.scss',
})
export class TrainderDetailsComponent {
	name: string = 'Arnold Schwarzenegger Pro Bodybuilding Profile';
	loader = inject(LoaderModalStore);
	trainerId: string = '';
	trainerInfo!: TrainerInfoDto;
	userInfo!: UserInfoDto;
	coursesOfTrainer: any;

	userStore = inject(UserStore);

	constructor(
		private trainerService: TrainerService,
		private activatedRoute: ActivatedRoute,
		private userService: UserService,
		private courseAgencyService: CourseAgencyService,
		private conversationService: ConversationService,
        private router: Router
	) {}

	ngOnInit() {
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

		patchState(this.loader, { isShow: true });
		this.activatedRoute.params.subscribe({
			next: (params: any) => {
				// console.log(params.id); // {id: '2', name: 'hoc'}
				this.trainerId = params.id;

				this.getTrainerInfo(this.trainerId);
			},
		});

		this.getCoursesOfTrainer();
	}

	private getUserInfo(userId: string) {
		this.userService.getUserInfo(userId).subscribe({
			next: (res: any) => {
				this.userInfo = { ...res.userInfo };
				if (this.userInfo.avatar === null) {
					this.userInfo.avatar = 'https://cdn-icons-png.flaticon.com/512/236/236832.png';
				}
			},
		});
	}

	private getTrainerInfo(trainerId: string) {
		this.trainerService.getTrainerInfo(trainerId).subscribe({
			next: (res: any) => {
				this.trainerInfo = { ...res };
				this.getUserInfo(this.trainerInfo.userId);
				if (this.trainerInfo.profilePicture === null) {
					this.trainerInfo.profilePicture =
						'https://cdn-icons-png.flaticon.com/512/236/236832.png';
				}
				patchState(this.loader, { isShow: false });
			},
		});
	}

	private getCoursesOfTrainer() {
		this.courseAgencyService.getCoursesOftrainer(this.trainerId).subscribe({
			next: (res: any) => {
				// console.log(res);
				this.coursesOfTrainer = res;

                console.log(res);
                
			},
		});
	}

	// create conversation
	onSendMessage() {
		const model = {
			senderId: this.userStore.id(),
			recieveId: this.userInfo.id,
		};

		patchState(this.loader, { isShow: true });
		this.conversationService.createConversation(model).subscribe({
			next: (res: any) => {
				// console.log(res);
				patchState(this.loader, { isShow: false });
                this.router.navigateByUrl('/chat/chat-list')
			},
			error: err => {
				// console.log(err);
				patchState(this.loader, { isShow: false });
			},
		});
	}
}
