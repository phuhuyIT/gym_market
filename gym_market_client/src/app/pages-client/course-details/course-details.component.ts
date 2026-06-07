import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit, OnDestroy, Renderer2 , ChangeDetectionStrategy } from '@angular/core';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CourseOptionService } from '../../page-agency/course-option.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { FormsModule } from '@angular/forms';
import { UserStore } from '../../stores/user.store';
import { CourseRatingService } from '../course-rating.service';
import { ToastService } from '../../shared/services/toast.service';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { CourseRegistrationService } from '../course-registration.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { GmCardComponent, GmButtonComponent, GmInputComponent } from '../../shared';
import { TrainerService } from '../../page-agency/trainer.service';
import { Trainer } from '../../core/models/trainer.model';
import {
	Course,
	CourseOption,
	CourseRating,
	CourseRatingCreateDto,
} from '../../core/models/course.model';
import { MOCK_FRIENDS } from '../../utilities/mock-data.const';
import { generateWorkoutPlans } from '../../utilities/workout-plans.const';

@Component({
    selector: 'app-course-details',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [CommonModule, FormsModule, DatePipe, DecimalPipe, GmCardComponent, GmButtonComponent, GmInputComponent],
    templateUrl: './course-details.component.html',
    styleUrl: './course-details.component.scss'
})
export class CourseDetailsComponent implements OnInit, OnDestroy {
	courseOptions: CourseOption[] = [];
	course: Course | null = null;
	loader = inject(LoaderModalStore);
	courseId: string = '';
	userStore = inject(UserStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private trainerService = inject(TrainerService);

	// rating
	rate: number = 0;
	comment: string = '';
	ratings: CourseRating[] = [];
	showAll: boolean = false;

	images: string[] = [];
	videos: string[] = [];

	// show image
	url: string | null = null;

	showPayment: boolean = false;

	// Redesigned UI State
	trainerInfo: Trainer | null = null;
	selectedDay: number = 1;
	activeExercise: any = null;
	isPlaying: boolean = false;
	timerValue: number = 0;
	totalSeconds: number = 0;
	timerInterval: any = null;
	isFavorited: boolean = false;
	volumeLevel: number = 80;
	isMuted: boolean = false;

	readonly mockFriends = MOCK_FRIENDS;

	dayPlans: any[] = [];

	constructor(
		private courseService: CourseAgencyService,
		private activatedRoute: ActivatedRoute,
		private courseOptionService: CourseOptionService,
		private router: Router,
		private courseRatingService: CourseRatingService,
		private renderer: Renderer2,
		private courseRegistrationService: CourseRegistrationService
	) {}

	ngOnInit() {
		this.activatedRoute.params.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: params => {
				this.courseId = params['id'];
				this.loadCourse(this.courseId);
				this.getCourseRating(this.courseId);
				this.getCourseOptions();
			},
		});
	}

	ngOnDestroy() {
		this.stopTimer();
	}

	private loadCourse(id: string) {
		patchState(this.loader, { isShow: true });
		this.courseService
			.getCourse(id)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.course = res;
					this.images = res.getFileDtos
						? res.getFileDtos.filter(c => c.typeFile === 'IMAGE').map(c => c.url)
						: [];
					this.videos = res.getFileDtos
						? res.getFileDtos.filter(c => c.typeFile === 'VIDEO').map(c => c.url)
						: [];

					// Fetch Trainer Info
					if (res.trainerId) {
						this.getTrainerInfo(res.trainerId);
					}

					// Generate workout plans
					this.dayPlans = generateWorkoutPlans(res.category);
					if (this.dayPlans.length > 0 && this.dayPlans[0].exercises.length > 0) {
						this.selectExercise(this.dayPlans[0].exercises[0]);
					}

					patchState(this.loader, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					this.router.navigateByUrl('/client/course-search');
				},
			});
	}

	private getTrainerInfo(trainerId: string) {
		this.trainerService
			.getTrainerInfo(trainerId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.trainerInfo = res;
					this.cdr.markForCheck();
				},
				error: () => {}
			});
	}

	private getCourseOptions() {
		patchState(this.loader, { isShow: true });
		this.courseOptionService
			.getCourseOptionsByCourseId(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.courseOptions = res;
					patchState(this.loader, { isShow: false });
					this.cdr.markForCheck();
				},
				error: err => {
					patchState(this.loader, { isShow: false });
				},
			});
	}

	private getCourseRating(id: string) {
		this.courseRatingService
			.getCourseRatings(id)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.ratings = res;
					this.cdr.markForCheck();
				},
			});
	}

	preventInvalidInput(event: KeyboardEvent): void {
		if (['e', 'E', '+', '-'].includes(event.key)) {
			event.preventDefault();
		}
	}

	addRating() {
		if (this.rate > 5 || this.rate < 0) {
			this.toastService.show('RatingValue must be between 0 and 5.', 'error');
			return;
		}

		const ratingDto: CourseRatingCreateDto = {
			courseId: this.courseId,
			studentId: this.userStore.studentId() ?? '',
			ratingValue: Number(this.rate),
			comment: this.comment,
		};

		patchState(this.loader, { isShow: true });
		this.courseRatingService
			.addRating(ratingDto)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.getCourseRating(this.courseId);
					this.rate = 0;
					this.comment = '';
					patchState(this.loader, { isShow: false });
					this.toastService.show('Rating added successfully');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loader, { isShow: false });
				},
			});
	}

	toggleShowAll() {
		this.showAll = !this.showAll;
	}

	showImage(url: string | null) {
		this.url = url;
		if (url) {
			this.renderer.addClass(document.body, 'no-scroll');
		} else {
			this.renderer.removeClass(document.body, 'no-scroll');
		}
	}

	onShowPayment(flag: boolean) {
		this.showPayment = flag;
	}

	addToCard(courseId: string) {
		this.courseId = courseId;
		patchState(this.loader, { isShow: true });
		this.courseRegistrationService
			.registerCourse(this.courseId, this.userStore.studentId() ?? '')
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.router.navigateByUrl('/client/course-registration');
					patchState(this.loader, { isShow: false });
					this.toastService.show('Enrolled successfully!');
				},
				error: () => {
					patchState(this.loader, { isShow: false });
					this.toastService.show('Enrollment failed. Please try again.', 'error');
				},
			});
	}

	// REDESIGNED CONTROLS
	selectDay(dayNum: number) {
		this.selectedDay = dayNum;
		this.stopTimer();
		const plan = this.dayPlans.find(d => d.dayNumber === dayNum);
		if (plan && plan.exercises.length > 0) {
			const active = plan.exercises.find((e: any) => e.status === 'active') || plan.exercises[0];
			this.selectExercise(active);
		}
		this.cdr.markForCheck();
	}

	selectExercise(ex: any) {
		this.stopTimer();
		this.activeExercise = ex;
		const secs = parseInt(ex.totalTime);
		this.timerValue = isNaN(secs) ? 30 : secs;
		this.totalSeconds = this.timerValue;
		this.isPlaying = false;
		this.isFavorited = false;
		this.cdr.markForCheck();
	}

	togglePlay() {
		if (this.isPlaying) {
			this.stopTimer();
		} else {
			this.startTimer();
		}
	}

	private startTimer() {
		this.isPlaying = true;
		if (this.timerValue <= 0) {
			this.timerValue = this.totalSeconds;
		}
		this.timerInterval = setInterval(() => {
			if (this.timerValue > 0) {
				this.timerValue--;
				if (this.activeExercise) {
					this.activeExercise.realTime = `${this.totalSeconds - this.timerValue}sec`;
				}
				this.cdr.markForCheck();
			} else {
				this.stopTimer();
				if (this.activeExercise) {
					this.activeExercise.status = 'done';
				}
				this.toastService.show('Exercise completed! Keep it up!');
				this.cdr.markForCheck();
			}
		}, 1000);
	}

	private stopTimer() {
		this.isPlaying = false;
		if (this.timerInterval) {
			clearInterval(this.timerInterval);
			this.timerInterval = null;
		}
	}

	toggleFavorite() {
		this.isFavorited = !this.isFavorited;
		this.cdr.markForCheck();
	}

	toggleMute() {
		this.isMuted = !this.isMuted;
		if (this.isMuted) {
			this.volumeLevel = 0;
		} else {
			this.volumeLevel = 80;
		}
		this.cdr.markForCheck();
	}

	adjustVolume(event: any) {
		this.volumeLevel = Number(event.target.value);
		this.isMuted = this.volumeLevel === 0;
		this.cdr.markForCheck();
	}

	getProgressPercent(): number {
		if (this.totalSeconds === 0) return 0;
		return ((this.totalSeconds - this.timerValue) / this.totalSeconds) * 100;
	}

	getActiveDayExercises() {
		const plan = this.dayPlans.find(d => d.dayNumber === this.selectedDay);
		return plan ? plan.exercises : [];
	}

	getActiveExerciseIndex() {
		if (!this.activeExercise) return 0;
		return this.getActiveDayExercises().indexOf(this.activeExercise) + 1;
	}

	getActiveDayExercisesCount() {
		return this.getActiveDayExercises().length;
	}
}

