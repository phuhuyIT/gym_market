import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit, Renderer2, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CourseAgencyService } from '../course-agency.service';
import { patchState } from '@ngrx/signals';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { LoaderModalStore } from '../../stores/loader.store';
import { Course } from '../../core/models/course.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { GmButtonComponent } from '../../shared';
import { ToastService } from '../../shared/services/toast.service';
import { DEFAULT_COURSE_THUMBNAIL_URL, DEFAULT_IMAGE_URL, formatDateToInput } from '../../utilities/defaults.const';
import { MAX_VIDEO_BYTES } from '../../utilities/upload.const';
import { FallbackSrcDirective } from '../../shared/directives/fallback-src.directive';

interface CourseTypeOption {
	value: string;
	label: string;
	icon: string;
	gradient: string;
}

@Component({
    selector: 'app-update-course',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [FormsModule, RouterLink, GmButtonComponent, DecimalPipe, FallbackSrcDirective],
    templateUrl: './update-course.component.html',
    styleUrl: './update-course.component.scss'
})
export class UpdateCourseComponent implements OnInit {
	model: Course = {
		courseId: '',
		trainerId: '',
		title: '',
		description: '',
		type: 'Yoga',
		category: 'Yoga',
		price: 0,
		additionalPrice: 0,
		startDate: '',
		endDate: '',
		duration: 0,
		maxParticipants: 0,
		status: 'Published',
		rating: 0,
		getFileDtos: [],
	};

	readonly descriptionLimit = 500;
	readonly courseStatuses = ['Draft', 'Published', 'Archived'] as const;
	readonly DEFAULT_COURSE_THUMBNAIL_URL = DEFAULT_COURSE_THUMBNAIL_URL;
	readonly DEFAULT_IMAGE_URL = DEFAULT_IMAGE_URL;

	// Full Tailwind class strings kept literal so the JIT compiler picks them up.
	readonly courseTypes: CourseTypeOption[] = [
		{ value: 'Yoga', label: 'Yoga', icon: '🧘', gradient: 'from-emerald-400 to-teal-500' },
		{ value: 'Cardio', label: 'Cardio', icon: '🏃', gradient: 'from-orange-400 to-rose-500' },
		{ value: 'Strength', label: 'Strength', icon: '🏋️', gradient: 'from-blue-500 to-indigo-600' },
		{ value: 'Pilates', label: 'Pilates', icon: '🤸', gradient: 'from-fuchsia-400 to-purple-500' },
		{ value: 'Stretching', label: 'Stretching', icon: '🙆', gradient: 'from-cyan-400 to-sky-500' },
		{ value: 'Cross fit', label: 'Cross fit', icon: '⚡', gradient: 'from-amber-400 to-orange-600' },
	];

	courseId: string = '';
	loading = false;
	loaderStore = inject(LoaderModalStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private route = inject(ActivatedRoute);
	private courseAgencyService = inject(CourseAgencyService);
	private router = inject(Router);
	private renderer = inject(Renderer2);

	dataImages: string[] = [];
	private imagesAdd: File[] = [];
	dataVideos: string[] = [];
	private videosAdd: File[] = [];
    url: string | null = null;

	get selectedType(): CourseTypeOption {
		return this.courseTypes.find(t => t.value === this.model.type) ?? this.courseTypes[0];
	}

	// Days between the chosen start and end dates (inclusive), for the schedule hint.
	get scheduleDays(): number {
		const start = new Date(this.model.startDate).getTime();
		const end = new Date(this.model.endDate).getTime();
		if (isNaN(start) || isNaN(end) || end < start) {
			return 0;
		}
		return Math.floor((end - start) / 86400000) + 1;
	}

	useScheduleLength() {
		if (this.scheduleDays > 0) {
			this.model.duration = this.scheduleDays;
		}
	}

	ngOnInit() {
		this.courseId = this.route.snapshot.params['id'];
		if (this.courseId) {
			this.getCourse(this.courseId);
		}
	}

	private getCourse(id: string) {
		patchState(this.loaderStore, { isShow: true });
		this.courseAgencyService.getCourse(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (res: Course) => {
				this.model = {
					...res,
					startDate: formatDateToInput(new Date(res.startDate)),
					endDate: formatDateToInput(new Date(res.endDate)),
				};
				this.dataImages = res.getFileDtos ? res.getFileDtos.filter((c) => c.typeFile === 'IMAGE').map((c) => c.url) : [];
				this.dataVideos = res.getFileDtos ? res.getFileDtos.filter((c) => c.typeFile === 'VIDEO').map((c) => c.url) : [];
				patchState(this.loaderStore, { isShow: false });
				this.cdr.markForCheck();
			},
			error: () => {
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Course not found', 'error');
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
					if (typeof data === 'string') this.dataImages.push(data);
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
				if (file.size > MAX_VIDEO_BYTES) {
					this.toastService.show(
						`"${file.name}" is too large (max ${MAX_VIDEO_BYTES / 1024 / 1024} MB)`,
						'error'
					);
					continue;
				}
				this.videosAdd.push(file);
				const videoUrl = URL.createObjectURL(file);
				this.dataVideos.push(videoUrl);
			}
			input.value = '';
		}
	}

	submit() {
		const form = new FormData();
		form.append('CourseId', this.model.courseId);
		form.append('Title', this.model.title);
		form.append('Description', this.model.description);
		form.append('Type', this.model.type);
		form.append('Category', this.model.category);
		form.append('Price', this.model.price.toString());
		form.append('AdditionalPrice', this.model.additionalPrice.toString());
		form.append('StartDate', this.model.startDate);
		form.append('EndDate', this.model.endDate);
		form.append('Duration', this.model.duration.toString());
		form.append('MaxParticipants', this.model.maxParticipants.toString());
		form.append('Status', this.model.status || 'Published');
		// No TrainerId: ownership never changes on update and the backend
		// keeps the current owner.

		for (let file of this.imagesAdd) form.append('Images', file);
		for (let file of this.videosAdd) form.append('Videos', file);

		this.loading = true;
		patchState(this.loaderStore, { isShow: true });

		this.courseAgencyService.updateCourse(form).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: () => {
				this.loading = false;
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Course updated successfully');
				this.router.navigateByUrl('/agency/courses');
			},
			error: err => {
				this.loading = false;
				patchState(this.loaderStore, { isShow: false });
				const message =
					err?.status === 413
						? 'Upload too large. Please use smaller video/image files.'
						: 'Failed to update course';
				this.toastService.show(message, 'error');
			},
		});
	}

    showImage(url: string | null) {
		this.url = url;
		if (url) this.renderer.addClass(document.body, 'no-scroll');
		else this.renderer.removeClass(document.body, 'no-scroll');
	}
}
