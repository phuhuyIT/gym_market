import { Component, inject } from '@angular/core';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CourseOptionService } from '../../page-agency/course-option.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { FormsModule } from '@angular/forms';
import { UserStore } from '../../stores/user.store';
import { CourseRatingService } from '../course-rating.service';
import { ErrorModalStore } from '../../stores/error-modal.store';

@Component({
	selector: 'app-course-details',
	standalone: true,
	imports: [FormsModule],
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

	// rating
	rate: number = 0;
	comment: string = '';
	ratings: any = [];
	showAll: boolean = false;

	constructor(
		private courseService: CourseAgencyService,
		private activatedRoute: ActivatedRoute,
		private courseOptionService: CourseOptionService,
		private router: Router,
		private courseRatingService: CourseRatingService
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
						// patchState(this.loader, { isShow: false });
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
}
