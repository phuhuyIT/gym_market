import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { CourseAgencyService } from '../course-agency.service';
import { PaymentService } from '../payment.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { NoticeModalStore } from '../../stores/notice.store';
import { UserStore } from '../../stores/user.store';
import { ToastService } from '../../shared/services/toast.service';
import { Course } from '../../core/models/course.model';
import { CancelPaymentDto } from '../../core/models/payment.model';

@Component({
    selector: 'app-payments',
    standalone: true,
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [CommonModule, RouterLink, FormsModule, DecimalPipe, DatePipe],
    templateUrl: './payments.component.html'
})
export class PaymentsComponent implements OnInit {
	courses: Course[] = [];
	allPayments: any[] = [];
	filteredPayments: any[] = [];

	searchString = '';
	statusFilter = '';
	// optional context when navigated from a specific course or student
	courseIdFilter = '';
	courseTitleFilter = '';
	studentIdFilter = '';
	studentNameFilter = '';

	// cancel modal state
	showCancelModal = false;
	paymentIdToCancel = '';
	cancelNote = '';

	loaderStore = inject(LoaderModalStore);
	noticeStore = inject(NoticeModalStore);
	userStore = inject(UserStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private paymentService = inject(PaymentService);
	private activatedRoute = inject(ActivatedRoute);
	private router = inject(Router);

	constructor(private courseAgencyService: CourseAgencyService) {}

	ngOnInit() {
		this.activatedRoute.queryParams.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {
			this.courseIdFilter = params['courseId'] || '';
			this.studentIdFilter = params['studentId'] || '';
			this.updateContextLabels();
			this.applyFilters();
		});
		this.loadData();
	}

	private updateContextLabels() {
		const course = this.courses.find(c => c.courseId === this.courseIdFilter);
		this.courseTitleFilter = course?.title ?? '';
		const payment = this.allPayments.find(p => p.studentId === this.studentIdFilter);
		this.studentNameFilter = payment?.studentName ?? '';
	}

	clearContextFilter() {
		this.courseIdFilter = '';
		this.courseTitleFilter = '';
		this.studentIdFilter = '';
		this.studentNameFilter = '';
		this.router.navigate([], { relativeTo: this.activatedRoute, queryParams: {} });
		this.applyFilters();
	}

	loadData() {
		patchState(this.loaderStore, { isShow: true });
		this.courseAgencyService
			.getCoursesOfTrainer(this.userStore.trainerId() ?? '')
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: courses => {
					this.courses = courses;
					this.updateContextLabels();
					this.loadPaymentsFromCourses();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load courses', 'error');
				}
			});
	}

	loadPaymentsFromCourses() {
		if (this.courses.length === 0) {
			this.allPayments = [];
			this.filteredPayments = [];
			patchState(this.loaderStore, { isShow: false });
			this.cdr.markForCheck();
			return;
		}

		const requests = this.courses.map(c =>
			this.paymentService.getPayments(c.courseId).pipe(
				catchError(() => of([]))
			)
		);

		forkJoin(requests).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (results) => {
				const list: any[] = [];
				results.forEach((payments, idx) => {
					const course = this.courses[idx];
					payments.forEach(p => {
						list.push({
							paymentId: p.paymentId,
							studentId: p.studentId,
							userId: p.userId,
							studentName: p.fullName || 'Student',
							courseId: course.courseId,
							courseTitle: course.title,
							paymentAmount: p.paymentAmount || course.price,
							paymentStatus: p.paymentStatus || 'Paid',
							paymentDate: p.paymentDate || p.createdAt || new Date().toISOString(),
							note: p.note
						});
					});
				});

				// Sort by registration date desc
				list.sort((a, b) => new Date(b.paymentDate).getTime() - new Date(a.paymentDate).getTime());
				this.allPayments = list;
				this.updateContextLabels();
				this.applyFilters();
				patchState(this.loaderStore, { isShow: false });
				this.cdr.markForCheck();
			},
			error: () => {
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Failed to load payments', 'error');
			}
		});
	}

	applyFilters() {
		let result = this.allPayments;

		if (this.courseIdFilter) {
			result = result.filter(p => p.courseId === this.courseIdFilter);
		}

		if (this.studentIdFilter) {
			result = result.filter(p => p.studentId === this.studentIdFilter);
		}

		if (this.searchString.trim()) {
			const q = this.searchString.toLowerCase();
			result = result.filter(p =>
				p.studentName.toLowerCase().includes(q) ||
				p.courseTitle.toLowerCase().includes(q)
			);
		}

		if (this.statusFilter) {
			result = result.filter(p => p.paymentStatus === this.statusFilter);
		}

		this.filteredPayments = result;
		this.cdr.markForCheck();
	}

	onSearch() {
		this.applyFilters();
	}

	onStatusFilter(status: string) {
		this.statusFilter = status;
		this.applyFilters();
	}

	okPayment(paymentId: string) {
		patchState(this.loaderStore, { isShow: true });
		this.paymentService
			.okPayment(paymentId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Payment approved');

					const payment = this.allPayments.find(p => p.paymentId === paymentId);
					if (payment) {
						payment.paymentStatus = 'Paid';
					}
					this.applyFilters();
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to approve payment', 'error');
				}
			});
	}

	onShowCancelModal(flag: boolean, paymentId: string) {
		this.showCancelModal = flag;
		this.paymentIdToCancel = paymentId;
		this.cancelNote = '';
	}

	cancelPaymentSubmit() {
		if (!this.cancelNote.trim()) {
			this.toastService.show('Please provide a reason/note for cancellation', 'error');
			return;
		}

		this.showCancelModal = false;
		patchState(this.loaderStore, { isShow: true });

		const model: CancelPaymentDto = {
			paymentId: this.paymentIdToCancel,
			note: this.cancelNote
		};

		this.paymentService
			.cancelPayment(model)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Payment canceled');

					const payment = this.allPayments.find(p => p.paymentId === this.paymentIdToCancel);
					if (payment) {
						payment.paymentStatus = 'Canceled';
						payment.note = this.cancelNote;
					}
					this.applyFilters();
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to cancel payment', 'error');
				}
			});
	}
}
