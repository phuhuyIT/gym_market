import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { TrainerService } from '../../page-agency/trainer.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { ConversationService } from '../../chat/conversation.service';
import { UserStore } from '../../stores/user.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Trainer } from '../../core/models/trainer.model';
import { Course } from '../../core/models/course.model';
import { CommonModule } from '@angular/common';
import { DEFAULT_AVATAR_IMAGE_URL, DEFAULT_COURSE_THUMBNAIL_URL } from '../../utilities/defaults.const';
import { FallbackSrcDirective } from '../../shared/directives/fallback-src.directive';

@Component({
    selector: 'app-trainer-details',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink, CommonModule, FallbackSrcDirective],
    templateUrl: './trainer-details.component.html',
    styleUrl: './trainer-details.component.scss'
})
export class TrainerDetailsComponent implements OnInit {
	loader = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	trainerId: string = '';
	trainerInfo: Trainer | null = null;
	coursesOfTrainer: Course[] = [];
	otherTrainers: Trainer[] = [];
	followedTrainersMap: { [id: string]: boolean } = {};
	bookmarkedCoursesMap: { [id: string]: boolean } = {};
	readonly DEFAULT_AVATAR_IMAGE_URL = DEFAULT_AVATAR_IMAGE_URL;
	readonly DEFAULT_COURSE_THUMBNAIL_URL = DEFAULT_COURSE_THUMBNAIL_URL;

	userStore = inject(UserStore);

	constructor(
		private trainerService: TrainerService,
		private activatedRoute: ActivatedRoute,
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
					this.getOtherTrainers();
				}
			},
		});
	}

	private getOtherTrainers() {
		this.trainerService
			.getTrainers()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.otherTrainers = res
						.filter(t => t.trainerId !== this.trainerId)
						.slice(0, 3);
					this.cdr.markForCheck();
				},
			});
	}

	toggleFollow(trainerId: string, event?: Event) {
		if (event) {
			event.stopPropagation();
			event.preventDefault();
		}
		this.followedTrainersMap[trainerId] = !this.followedTrainersMap[trainerId];
		this.cdr.markForCheck();
	}

	isFollowed(trainerId: string): boolean {
		return !!this.followedTrainersMap[trainerId];
	}

	toggleCourseBookmark(courseId: string, event?: Event) {
		if (event) {
			event.stopPropagation();
			event.preventDefault();
		}
		this.bookmarkedCoursesMap[courseId] = !this.bookmarkedCoursesMap[courseId];
		this.cdr.markForCheck();
	}

	isCourseBookmarked(courseId: string): boolean {
		return !!this.bookmarkedCoursesMap[courseId];
	}

	private getTrainerInfo(trainerId: string) {
		patchState(this.loader, { isShow: true });
		this.trainerService
			.getTrainerInfo(trainerId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.trainerInfo = res;
					patchState(this.loader, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loader, { isShow: false });
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

	getCourseThumbnail(course: Course): string {
		return course.getFileDtos?.find(file => file.typeFile === 'IMAGE')?.url || DEFAULT_COURSE_THUMBNAIL_URL;
	}

	onSendMessage() {
		if (!this.trainerInfo) return;

		const senderId = this.userStore.id();
		if (!senderId) {
			this.router.navigateByUrl('/login');
			return;
		}

		const model = {
			recieveId: this.trainerInfo.userId,
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
