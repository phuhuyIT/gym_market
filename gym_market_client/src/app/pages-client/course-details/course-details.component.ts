import { Component, inject, Renderer2 } from '@angular/core';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CourseOptionService } from '../../page-agency/course-option.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { FormsModule } from '@angular/forms';
import { UserStore } from '../../stores/user.store';
import { CourseRatingService } from '../course-rating.service';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { DatePipe, DecimalPipe } from '@angular/common';
import { CouresRegistrationService } from '../coures-registration.service';
import { NoticeModalStore } from '../../stores/notice.store';

@Component({
	selector: 'app-course-details',
	standalone: true,
	imports: [FormsModule, DatePipe, DecimalPipe],
	templateUrl: './course-details.component.html',
	styleUrl: './course-details.component.scss',
})
export class CourseDetailsComponent {
	courseOptions: any;
	course: any;
	loader = inject(LoaderModalStore);
	courseId: string = '';
	userStore = inject(UserStore);
	errorModal = inject(ErrorModalStore);
	noticeStore = inject(NoticeModalStore);

	// rating
	rate: number = 0;
	comment: string = '';
	ratings: any = [];
	showAll: boolean = false;

	images: any = [];
	videos: any = [];

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
		private couresRegistrationService: CouresRegistrationService
	) {}

	ngOnInit() {
		this.courseOptions = [];

		this.getCourse();
		this.getCourseOPtion();
		this.getCourseRating();
	}

	private getCourse() {
		patchState(this.loader, { isShow: true });
		this.activatedRoute.params.subscribe({
			next: (params: any) => {
				// console.log(params.id); // {id: '2', name: 'hoc'}
				this.courseId = params.id;
				this.courseService.getCourse(params.id).subscribe({
					next: (res: any) => {
						this.course = res;
						this.images = res.getFileDtos
							.filter((c: any) => c.typeFile === 'IMAGE')
							.map((c: any) => c.url);
						this.videos = res.getFileDtos
							.filter((c: any) => c.typeFile === 'VIDEO')
							.map((c: any) => c.url);
						patchState(this.loader, { isShow: false });
					},
					error: err => {
						this.router.navigateByUrl('/client/home-client');
					},
				});
			},
		});
	}

	private getCourseOPtion() {
		patchState(this.loader, { isShow: true });
		this.courseOptionService.getCourseOptionsOftrainer().subscribe({
			next: (res: any) => {
				// console.log(res);
				this.courseOptions = res;
				// patchState(this.loader, { isShow: true });
				patchState(this.loader, { isShow: false });
			},
		});
	}

	private getCourseRating() {
		patchState(this.loader, { isShow: true });
		this.courseRatingService.getCourseRatings(this.courseId).subscribe({
			next: (res: any) => {
				// console.log(res);
				this.ratings = res;
				patchState(this.loader, { isShow: false });
			},
		});
	}

	preventInvalidInput(event: KeyboardEvent): void {
		// Nếu ký tự là 'e', '+', '-', hoặc '.'
		if (['e', 'E', '+', '-'].includes(event.key)) {
			event.preventDefault();
		}
	}

	addRating() {
		const t = {
			ratingId: crypto.randomUUID(),
			courseId: this.courseId,
			studentId: this.userStore.studentId(),
			ratingValue: this.rate,
			reviewComment: this.comment,
		};

		this.ratings.push(t);
		// console.log(t);

		if (this.rate > 5 || this.rate < 0) {
			patchState(this.errorModal, {
				errors: ['RatingValue must be between 0 and 5.'],
				isShow: true,
			});

			return;
		}

		patchState(this.loader, { isShow: true });
		this.courseRatingService.addRating(t).subscribe({
			next: (res: any) => {
				// console.log(res);
				patchState(this.loader, { isShow: false });
			},
			error: err => {
				console.log(err);
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

	addToCard(courseId: string ) {
        this.courseId = courseId;

		patchState(this.loader, { isShow: true });
		this.couresRegistrationService
			.registerCourse(this.courseId, this.userStore.studentId())
			.subscribe({
				next: (res: any) => {
                    this.router.navigateByUrl('/client/course-registration');
					console.log(123);
					patchState(this.loader, { isShow: false });
					patchState(this.noticeStore, {
						isShow: true,
						message: 'Register course successfully!',
					});
				},
				error: err => {
                    this.router.navigateByUrl('/client/course-registration');
					console.log(err);
					// patchState(this.loader, { isShow: false });
					// patchState(this.errorModal, {
					// 	isShow: true,
					// 	errors: ['Register course failed!'],
					// });
					patchState(this.loader, { isShow: false });
					patchState(this.noticeStore, {
						isShow: true,
						message: 'Register course successfully!',
					});
				},
			});
	}
}
