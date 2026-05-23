import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { TrainerService } from '../../page-agency/trainer.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { UserService } from '../../user/user.service';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { ConversationService } from '../../chat/conversation.service';
import { UserStore } from '../../stores/user.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Trainer } from '../../core/models/trainer.model';
import { Course } from '../../core/models/course.model';
import { CommonModule } from '@angular/common';
import { UserInfo } from '../../core/models/auth.model';
import { GmCardComponent, GmButtonComponent } from '../../shared';
import { DEFAULT_AVATAR_URL } from '../../utilities/defaults.const';

@Component({
    selector: 'app-trainer-details',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink, CommonModule, GmCardComponent, GmButtonComponent],
    templateUrl: './trainer-details.component.html',
    styleUrl: './trainer-details.component.scss'
})
export class TrainerDetailsComponent implements OnInit {
	loader = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	trainerId: string = '';
	trainerInfo: Trainer | null = null;
	userInfo: UserInfo | null = null;
	coursesOfTrainer: Course[] = [];

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
		this.activatedRoute.params.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: params => {
				this.trainerId = params['id'];
				if (this.trainerId) {
					this.getTrainerInfo(this.trainerId);
					this.getCoursesOfTrainer();
				}
			},
		});
	}

	private getTrainerInfo(trainerId: string) {
		patchState(this.loader, { isShow: true });
		this.trainerService
			.getTrainerInfo(trainerId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.trainerInfo = res;
					if (!this.trainerInfo.profilePicture) {
						this.trainerInfo.profilePicture = DEFAULT_AVATAR_URL;
					}
					this.getUserInfo(res.userId);
					patchState(this.loader, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loader, { isShow: false });
				},
			});
	}

	private getUserInfo(userId: string) {
		this.userService
			.getUserInfo(userId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.userInfo = res.userInfo;
					this.cdr.markForCheck();
				},
			});
	}

	private getCoursesOfTrainer() {
		this.courseAgencyService
			.getCoursesOfTrainer(this.trainerId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.coursesOfTrainer = res;
					this.cdr.markForCheck();
				},
			});
	}

	onSendMessage() {
		if (!this.trainerInfo) return;

		const studentId = this.userStore.studentId();
		if (!studentId) {
			this.router.navigateByUrl('/login');
			return;
		}

		const model = {
			trainerId: this.trainerId,
			studentId: studentId,
		};

		patchState(this.loader, { isShow: true });
		this.conversationService
			.createConversation(model)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.loader, { isShow: false });
					this.router.navigateByUrl('/chat/chat-list');
				},
				error: () => {
					patchState(this.loader, { isShow: false });
				},
			});
	}
}
