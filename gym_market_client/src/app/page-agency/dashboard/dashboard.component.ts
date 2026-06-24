import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CourseAgencyService } from '../course-agency.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { UserStore } from '../../stores/user.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Course } from '../../core/models/course.model';
import { PaymentService } from '../payment.service';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { PaymentMetrics } from '../../core/models/payment.model';

@Component({
    selector: 'app-dashboard',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink, CommonModule],
    templateUrl: './dashboard.component.html',
    styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
	courses: Course[] = [];
	loaderStore = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private paymentService = inject(PaymentService);

	userStore = inject(UserStore);

	metrics: PaymentMetrics = this.emptyMetrics();

	constructor(private courseAgencyService: CourseAgencyService) {}

	ngOnInit() {
		this.loadDashboard();
	}

	loadDashboard() {
		patchState(this.loaderStore, { isShow: true });
		const trainerId = this.userStore.trainerId();
		const courses$ = trainerId
			? this.courseAgencyService.getCoursesOfTrainer(trainerId).pipe(catchError(() => of([] as Course[])))
			: of([] as Course[]);

		forkJoin({
			courses: courses$,
			metrics: this.paymentService.getPaymentMetrics(6).pipe(catchError(() => of(this.emptyMetrics())))
		})
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.courses = res.courses;
					this.metrics = res.metrics;
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
			});
	}

	private emptyMetrics(): PaymentMetrics {
		return {
			totalPaidRevenue: 0,
			pendingAmount: 0,
			paidCount: 0,
			pendingCount: 0,
			canceledCount: 0,
			expiredCount: 0,
			uniquePaidStudentCount: 0,
			revenueByCourse: [],
			recentPaidPayments: []
		};
	}

	get displayEnrollments() {
		return this.metrics.recentPaidPayments;
	}

	get popularCourses() {
		if (this.metrics.revenueByCourse.length > 0) {
			return this.metrics.revenueByCourse.slice(0, 4);
		}

		return [];
	}

	get computedEarnings(): number {
		return this.metrics.totalPaidRevenue;
	}

	get computedStudents(): number {
		return this.metrics.uniquePaidStudentCount;
	}
}
