import { ChangeDetectorRef, Component, inject, DestroyRef , ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DecimalPipe } from '@angular/common';
import { CourseAgencyService } from '../course-agency.service';
import { jwtDecode } from 'jwt-decode';
import { Router, RouterLink } from '@angular/router';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UserTokenPayload } from '../../core/models/auth.model';
import { GmButtonComponent } from '../../shared';
import { formatDateToInput } from '../../utilities/defaults.const';
import { ToastService } from '../../shared/services/toast.service';
import { AccountService } from '../../guest/account.service';

interface CourseTypeOption {
	value: string;
	label: string;
	icon: string;
	gradient: string;
}

@Component({
    selector: 'app-add-course',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [FormsModule, RouterLink, GmButtonComponent, DecimalPipe],
    templateUrl: './add-course.component.html',
    styleUrl: './add-course.component.scss'
})
export class AddCourseComponent {
	model = {
		title: '',
		description: '',
		type: 'Yoga',
		category: 'Yoga',
		price: 0,
		additionalPrice: 0,
		startDate: formatDateToInput(new Date()),
		endDate: formatDateToInput(new Date()),
		duration: 0,
		maxParticipants: 0,
	};

	readonly descriptionLimit = 500;

	// Full Tailwind class strings kept literal so the JIT compiler picks them up.
	readonly courseTypes: CourseTypeOption[] = [
		{ value: 'Yoga', label: 'Yoga', icon: '🧘', gradient: 'from-emerald-400 to-teal-500' },
		{ value: 'Cardio', label: 'Cardio', icon: '🏃', gradient: 'from-orange-400 to-rose-500' },
		{ value: 'Strength', label: 'Strength', icon: '🏋️', gradient: 'from-blue-500 to-indigo-600' },
		{ value: 'Pilates', label: 'Pilates', icon: '🤸', gradient: 'from-fuchsia-400 to-purple-500' },
		{ value: 'Stretching', label: 'Stretching', icon: '🙆', gradient: 'from-cyan-400 to-sky-500' },
		{ value: 'Cross fit', label: 'Cross fit', icon: '⚡', gradient: 'from-amber-400 to-orange-600' },
	];

	loading = false;
	loaderStore = inject(LoaderModalStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(
		private courseAgencyService: CourseAgencyService,
		private router: Router,
		private accountService: AccountService
	) {}

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

	submit() {
		const token = this.accountService.token;
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
					this.router.navigateByUrl('/agency/courses');
				},
				error: err => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to create course', 'error');
					this.cdr.markForCheck();
				},
			});
	}
}
