import { Component, DestroyRef, inject, OnInit, Renderer2 } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CourseAgencyService } from '../course-agency.service';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { patchState } from '@ngrx/signals';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LoaderModalStore } from '../../stores/loader.store';
import { NoticeModalStore } from '../../stores/notice.store';
import { Course } from '../../core/models/course.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
	selector: 'app-update-course',
	standalone: true,
	imports: [ReactiveFormsModule],
	templateUrl: './update-course.component.html',
	styleUrl: './update-course.component.scss',
})
export class UpdateCourseComponent implements OnInit {
	course!: Course;
	errorModalStore = inject(ErrorModalStore);
	form!: FormGroup;
	loaderStore = inject(LoaderModalStore);
	errorStore = inject(ErrorModalStore);
	noticeStore = inject(NoticeModalStore);
	private destroyRef = inject(DestroyRef);
	private activatedRoute = inject(ActivatedRoute);
	private courseAgencyService = inject(CourseAgencyService);
	private router = inject(Router);
	private formBuilder = inject(FormBuilder);
	private renderer = inject(Renderer2);

	dataImages: string[] = []; // lưu data của images để hiển thị trên view
	private imagesAdd: File[] = []; // chứa đối tượng file để tải lên server

	dataVideos: string[] = []; // lưu data của video để hiển thị trên view
	private videosAdd: File[] = []; // chứa đối tượng file để tải lên server

    url: string | null = null;

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

		this.activatedRoute.params.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (params) => {
				if (params['id']) {
					this.getCourse(params['id']);
				}
			},
		});
	}

	private initCourse() {
		this.form.patchValue({
			courseId: this.course.courseId,
			trainerId: this.course.trainerId,
			title: this.course.title,
			description: this.course.description,
			type: this.course.type,
			category: this.course.category,
			price: this.course.price,
			additionalPrice: this.course.additionalPrice,
			startDate: this.formatDate(new Date(this.course.startDate)),
			endDate: this.formatDate(new Date(this.course.endDate)),
			duration: this.course.duration,
			maxParticipants: this.course.maxParticipants,
		});
	}

	private formatDate(date: Date): string {
		const year = date.getFullYear();
		const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Tháng bắt đầu từ 0
		const day = date.getDate().toString().padStart(2, '0');
		return `${year}-${month}-${day}`;
	}

	private getCourse(id: string) {
		this.courseAgencyService.getCourse(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (res: Course) => {
				const images = res.getFileDtos.filter((c) => c.typeFile === 'IMAGE').map((c) => c.url);
				const videos = res.getFileDtos.filter((c) => c.typeFile === 'VIDEO').map((c) => c.url);
				this.dataImages = images;
				this.dataVideos = videos;
				this.course = res;
				this.initCourse();
			},
			error: () => {
				patchState(this.errorModalStore, {
					errors: ['Course not found'],
					isShow: true,
				});
				this.router.navigateByUrl('/agency/courses');
			},
		});
	}

	onSlectImage(event: Event) {
		const input = event.target as HTMLInputElement;
		if (input.files && input.files.length > 0) {
			for (let i = 0; i < input.files.length; i++) {
				const file = input.files[i];
				this.imagesAdd.push(file);

				const reader = new FileReader();
				reader.readAsDataURL(file);
				reader.onload = e => {
					const data = e.target?.result;
					if (typeof data === 'string') {
						this.dataImages.push(data);
					}
				};
			}
		}
	}

	onClearImages(input: HTMLInputElement) {
		this.dataImages = [];
		this.imagesAdd = [];

		input.value = '';
	}

	onClearVideo(input: HTMLInputElement) {
		this.dataVideos = [];
		this.videosAdd = [];

		input.value = '';
	}

	onSlectVideo(event: Event) {
		const input = event.target as HTMLInputElement;
		if (input.files && input.files.length > 0) {
			for (let i = 0; i < input.files.length; i++) {
				const file = input.files[i];
				this.videosAdd.push(file);

				const videoUrl = URL.createObjectURL(file);
				this.dataVideos.push(videoUrl);
			}
		}
	}

	submit() {
		const form = new FormData();
		form.append('CourseId', this.form.controls['courseId'].value);
		form.append('Title', this.form.controls['title'].value);
		form.append('Description', this.form.controls['description'].value);
		form.append('Type', this.form.controls['type'].value);
		form.append('Category', this.form.controls['category'].value);
		form.append('Price', this.form.controls['price'].value);
		form.append('AdditionalPrice', this.form.controls['additionalPrice'].value);
		form.append('StartDate', this.form.controls['startDate'].value);
		form.append('EndDate', this.form.controls['endDate'].value);
		form.append('Duration', this.form.controls['duration'].value);
		form.append('MaxParticipants', this.form.controls['maxParticipants'].value);
		form.append('TrainerId', this.form.controls['trainerId'].value);

		for (let file of this.imagesAdd) {
			form.append('Images', file);
		}

		for (let file of this.videosAdd) {
			form.append('Videos', file);
		}

		patchState(this.loaderStore, { isShow: true });

		this.courseAgencyService.updateCourse(form as any).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: () => {
				patchState(this.loaderStore, { isShow: false });
				patchState(this.noticeStore, { isShow: true, message: 'Update successful' });
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

    showImage(url: string | null) {
		this.url = url;

		if (url) {
			this.renderer.addClass(document.body, 'no-scroll');
		} else {
			this.renderer.removeClass(document.body, 'no-scroll');
		}
	}
}
