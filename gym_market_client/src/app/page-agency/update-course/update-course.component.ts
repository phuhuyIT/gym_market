import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CourseAgencyService } from '../course-agency.service';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { patchState } from '@ngrx/signals';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LoaderModalStore } from '../../stores/loader.store';
import { NoticeModalStore } from '../../stores/notice.store';

@Component({
	selector: 'app-update-course',
	standalone: true,
	imports: [ReactiveFormsModule],
	templateUrl: './update-course.component.html',
	styleUrl: './update-course.component.scss',
})
export class UpdateCourseComponent {
	course: any;
	errorModalStore = inject(ErrorModalStore);
	form!: FormGroup;
	loaderStore = inject(LoaderModalStore);
	errorStore = inject(ErrorModalStore);
	noticeStore = inject(NoticeModalStore);

	constructor(
		private activatedRoute: ActivatedRoute,
		private courseAgencyService: CourseAgencyService,
		private router: Router,
		private formBuilder: FormBuilder
	) {}

	ngOnInit() {
		this.form = this.formBuilder.group({
			courseId: [''],
			trainerId: [''],
			title: ['', [Validators.required]],
			description: ['', [Validators.required]],
			type: ['Yoga', [Validators.required]],
			category: ['Yoga', [Validators.required]],
			price: [0, [Validators.required]],
			additionalPrice: [0, [Validators.required]],
			startDate: [this.formatDate(new Date()), [Validators.required]],
			endDate: [this.formatDate(new Date()), [Validators.required]],
			duration: [0, [Validators.required]],
			maxParticipants: [0, [Validators.required]],
		});

		this.activatedRoute.params.subscribe({
			next: (params: any) => {
				// console.log(params.id);
				this.getCourse(params.id);
			},
		});
	}

	private initCourse() {
		this.form = this.formBuilder.group({
			courseId: [this.course.courseId],
			trainerId: [this.course.trainerId],
			title: [this.course.title, [Validators.required]],
			description: [this.course.description, [Validators.required]],
			type: [this.course.type, [Validators.required]],
			category: [this.course.category, [Validators.required]],
			price: [this.course.price, [Validators.required]],
			additionalPrice: [this.course.additionalPrice, [Validators.required]],
			startDate: [this.formatDate(new Date(this.course.startDate)), [Validators.required]],
			endDate: [this.formatDate(new Date(this.course.endDate)), [Validators.required]],
			duration: [this.course.duration, [Validators.required]],
			maxParticipants: [this.course.maxParticipants, [Validators.required]],
		});
	}

	private formatDate(date: Date): string {
		const year = date.getFullYear();
		const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Tháng bắt đầu từ 0
		const day = date.getDate().toString().padStart(2, '0');
		return `${year}-${month}-${day}`;
	}

	private getCourse(id: string) {
		this.courseAgencyService.getCourse(id).subscribe({
			next: (res: any) => {
				// console.log(res);
				this.course = res;
				this.initCourse();
			},
			error: err => {
				patchState(this.errorModalStore, {
					errors: ['Không tìm thấy course'],
					isShow: true,
				});
				this.router.navigateByUrl('/agency/courses');
			},
		});
	}

	submit() {
		patchState(this.loaderStore, { isShow: true });

		this.courseAgencyService.updateCourse(this.form.value).subscribe({
			next: (res: any) => {
				patchState(this.loaderStore, { isShow: false });
				patchState(this.noticeStore, { isShow: true, message: 'Cập nhật thành công' });
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				let result = [];
				for (const key in err.error.errors) {
					if (err.error.errors.hasOwnProperty(key)) {
						result.push(`${key}: ${err.error.errors[key][0]}\n`);
					}
				}
				patchState(this.errorModalStore, { errors: result, isShow: true });
			},
		});
	}
}
