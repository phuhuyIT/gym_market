import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, OnInit, inject } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { PaymentService } from '../../page-agency/payment.service';
import { PaymentMetrics } from '../../core/models/payment.model';

@Component({
	selector: 'app-admin-dashboard',
	imports: [CommonModule, DecimalPipe],
	templateUrl: './admin-dashboard.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminDashboardComponent implements OnInit {
	courseCount = 0;
	metrics: PaymentMetrics = {
		totalPaidRevenue: 0,
		pendingAmount: 0,
		paidCount: 0,
		pendingCount: 0,
		canceledCount: 0,
		expiredCount: 0,
		uniquePaidStudentCount: 0,
		revenueByCourse: [],
		recentPaidPayments: [],
	};

	private courses = inject(CourseAgencyService);
	private payments = inject(PaymentService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit(): void {
		forkJoin({
			courses: this.courses.getCoursesPaged(1, 1, '', '').pipe(catchError(() => of({ totalCount: 0 } as any))),
			metrics: this.payments.getPaymentMetrics(8).pipe(catchError(() => of(this.metrics))),
		})
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe(({ courses, metrics }) => {
				this.courseCount = courses.totalCount ?? courses.items?.length ?? 0;
				this.metrics = metrics;
				this.cdr.markForCheck();
			});
	}
}
