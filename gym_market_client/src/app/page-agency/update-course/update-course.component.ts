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

	dataImages: any = []; // lưu data của images để hiển thị trên view
	private imagesAdd: any = []; // chứa đối tượng file để tải lên server

	dataVideos: any = []; // lưu data của video để hiển thị trên view
	private videosAdd: any = []; // chứa đối tượng file để tải lên server

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
				const images = res.getFileDtos.filter((c: any) => c.typeFile === 'IMAGE').map((c: any) => c.url);
				const videos = res.getFileDtos.filter((c: any) => c.typeFile === 'VIDEO').map((c: any) => c.url);
				this.dataImages = images;
				this.dataVideos = videos;
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

	onSlectImage(event: any) {
		if (event.target.files.length > 0) {
			for (let file of event.target.files) {
				this.imagesAdd.push(file);

				const reader = new FileReader();
				reader.readAsDataURL(file);
				reader.onload = event => {
					const data = event.target?.result;
					this.dataImages.push(data);
				};
			}
		}
	}

	onClearImages(input: any) {
		this.dataImages = [];
		this.imagesAdd = [];

		input.value = '';
	}

	onClearVideo(input: any) {
		this.dataVideos = [];
		this.videosAdd = [];

		input.value = '';
	}

	onSlectVideo(event: any) {
		if (event.target.files.length > 0) {
			for (let file of event.target.files) {
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

		this.courseAgencyService.updateCourse(form).subscribe({
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
