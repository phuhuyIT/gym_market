import { DatePipe, CommonModule } from '@angular/common';
import { Component, inject, OnInit, DestroyRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CourseAgencyService } from '../course-agency.service';
import { jwtDecode } from 'jwt-decode';
import { Router, RouterLink } from '@angular/router';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UserTokenPayload } from '../../core/models/auth.model';
import { GmInputComponent, GmButtonComponent } from '../../shared';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-add-course',
	standalone: true,
	imports: [CommonModule, FormsModule, RouterLink, GmInputComponent, GmButtonComponent],
	templateUrl: './add-course.component.html',
	styleUrl: './add-course.component.scss',
})
export class AddCourseComponent implements OnInit {
	model = {
		title: '',
		description: '',
		type: 'Yoga',
		category: 'Yoga',
		price: 0,
		additionalPrice: 0,
		startDate: this.formatDate(new Date()),
		endDate: this.formatDate(new Date()),
		duration: 0,
		maxParticipants: 0,
	};
	
	loading = false;
	loaderStore = inject(LoaderModalStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);

	constructor(
		private courseAgencyService: CourseAgencyService,
		private router: Router
	) {}

	ngOnInit() {}

	private formatDate(date: Date): string {
		const year = date.getFullYear();
		const month = (date.getMonth() + 1).toString().padStart(2, '0');
		const day = date.getDate().toString().padStart(2, '0');
		return `${year}-${month}-${day}`;
	}

	submit() {
		const token = localStorage.getItem('gym-token');
		if (token === null) {
			this.toastService.show('Unauthorized. Please login.', 'error');
			return;
		}

		if (!this.model.title || !this.model.description) {
			this.toastService.show('Please fill in title and description', 'error');
			return;
		}

		const decoded = jwtDecode<UserTokenPayload & { trainerId: string }>(token);
		const trainerId = decoded.trainerId;
		const courseId = crypto.randomUUID();

		this.loading = true;
		patchState(this.loaderStore, { isShow: true });

		const submitModel = { ...this.model, trainerId, courseId };
		this.courseAgencyService
			.addCourse(submitModel)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Course created successfully');
					this.router.navigateByUrl('/agency/course-list');
				},
				error: err => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to create course', 'error');
				},
			});
	}
}
//
