import { Component, DestroyRef, ElementRef, inject, OnInit, Renderer2, ViewChild } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CourseAgencyService } from '../course-agency.service';
import { patchState } from '@ngrx/signals';
import { FormsModule } from '@angular/forms';
import { LoaderModalStore } from '../../stores/loader.store';
import { Course } from '../../core/models/course.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule, DatePipe } from '@angular/common';
import { GmInputComponent, GmButtonComponent } from '../../shared';
import { ToastService } from '../../shared/services/toast.service';

@Component({
    selector: 'app-update-course',
    imports: [CommonModule, FormsModule, RouterLink, GmInputComponent, GmButtonComponent, DatePipe],
    templateUrl: './update-course.component.html',
    styleUrl: './update-course.component.scss'
})
export class UpdateCourseComponent implements OnInit {
	model: any = {
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
	};
	
	courseId: string = '';
	loading = false;
	loaderStore = inject(LoaderModalStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private route = inject(ActivatedRoute);
	private courseAgencyService = inject(CourseAgencyService);
	private router = inject(Router);
	private renderer = inject(Renderer2);

	dataImages: string[] = [];
	private imagesAdd: File[] = [];
	dataVideos: string[] = [];
	private videosAdd: File[] = [];
    url: string | null = null;

	ngOnInit() {
		this.courseId = this.route.snapshot.params['id'];
		if (this.courseId) {
			this.getCourse(this.courseId);
		}
	}

	private formatDate(date: Date): string {
		const year = date.getFullYear();
		const month = (date.getMonth() + 1).toString().padStart(2, '0');
		const day = date.getDate().toString().padStart(2, '0');
		return `${year}-${month}-${day}`;
	}

	private getCourse(id: string) {
		patchState(this.loaderStore, { isShow: true });
		this.courseAgencyService.getCourse(id).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (res: Course) => {
				this.model = {
					...res,
					startDate: this.formatDate(new Date(res.startDate)),
					endDate: this.formatDate(new Date(res.endDate)),
				};
				this.dataImages = res.getFileDtos ? res.getFileDtos.filter((c) => c.typeFile === 'IMAGE').map((c) => c.url) : [];
				this.dataVideos = res.getFileDtos ? res.getFileDtos.filter((c) => c.typeFile === 'VIDEO').map((c) => c.url) : [];
				patchState(this.loaderStore, { isShow: false });
			},
			error: () => {
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Course not found', 'error');
				this.router.navigateByUrl('/agency/course-list');
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
				this.videosAdd.push(file);
				const videoUrl = URL.createObjectURL(file);
				this.dataVideos.push(videoUrl);
			}
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
		form.append('TrainerId', this.model.trainerId);

		for (let file of this.imagesAdd) form.append('Images', file);
		for (let file of this.videosAdd) form.append('Videos', file);

		this.loading = true;
		patchState(this.loaderStore, { isShow: true });

		this.courseAgencyService.updateCourse(form as any).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: () => {
				this.loading = false;
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Course updated successfully');
				this.router.navigateByUrl('/agency/course-list');
			},
			error: () => {
				this.loading = false;
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Failed to update course', 'error');
			},
		});
	}

    showImage(url: string | null) {
		this.url = url;
		if (url) this.renderer.addClass(document.body, 'no-scroll');
		else this.renderer.removeClass(document.body, 'no-scroll');
	}
}
