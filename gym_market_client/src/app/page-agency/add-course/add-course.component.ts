import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, DestroyRef } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CourseAgencyService } from '../course-agency.service';
import { jwtDecode } from 'jwt-decode';
import { Router, RouterLink } from '@angular/router';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UserTokenPayload } from '../../core/models/auth.model';

@Component({
	selector: 'app-add-course',
	standalone: true,
	imports: [ReactiveFormsModule],
	templateUrl: './add-course.component.html',
	styleUrl: './add-course.component.scss',
})
export class AddCourseComponent implements OnInit {
	form!: FormGroup;
	errorModalStore = inject(ErrorModalStore);
	loaderStore = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);

	constructor(
		private formBuilder: FormBuilder,
		private courseAgencyService: CourseAgencyService,
		private router: Router
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
	}

	private formatDate(date: Date): string {
		const year = date.getFullYear();
		const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Month starts from 0
		const day = date.getDate().toString().padStart(2, '0');
		return `${year}-${month}-${day}`;
	}

	submit() {
		const token = localStorage.getItem('gym-token');
		if (token === null) {
			return;
		}
		const decoded = jwtDecode<UserTokenPayload & { trainerId: string }>(token);
		const trainerId = decoded.trainerId;
		const courseId = crypto.randomUUID();

		const model = { ...this.form.value, trainerId, courseId };
		this.courseAgencyService.addCourse(model).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (_res) => {
				this.router.navigateByUrl('/agency/courses');
			},
			error: err => {
				let result: string[] = [];
				for (const key in err.error.errors) {
					if (Object.prototype.hasOwnProperty.call(err.error.errors, key)) {
						result.push(`${key}: ${err.error.errors[key][0]}\n`);
					}
				}
				patchState(this.errorModalStore, { errors: result, isShow: true });
				patchState(this.loaderStore, { isShow: false });
			},
		});
	}
}
//
