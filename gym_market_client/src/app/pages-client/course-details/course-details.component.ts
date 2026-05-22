import { Component, DestroyRef, inject, OnInit, Renderer2 } from '@angular/core';
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
import {
	Course,
	CourseOption,
	CourseRating,
	CourseRatingCreateDto,
} from '../../core/models/course.model';

@Component({
    selector: 'app-course-details',
    imports: [CommonModule, FormsModule, DatePipe, DecimalPipe, GmCardComponent, GmButtonComponent, GmInputComponent],
    templateUrl: './course-details.component.html',
    styleUrl: './course-details.component.scss'
})
export class CourseDetailsComponent implements OnInit {
	courseOptions: CourseOption[] = [];
	course: Course | null = null;
	loader = inject(LoaderModalStore);
	courseId: string = '';
	userStore = inject(UserStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);

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
					patchState(this.loader, { isShow: false });
				},
				error: () => {
					this.router.navigateByUrl('/client/course-search');
				},
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
}
