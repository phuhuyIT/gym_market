import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, effect, inject, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { PaymentService } from '../payment.service';
import { NotificationService } from '../../core/services/notification.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { NoticeModalStore } from '../../stores/notice.store';
import { UserStore } from '../../stores/user.store';
import { ToastService } from '../../shared/services/toast.service';
import { CancelPaymentDto, Payment } from '../../core/models/payment.model';
import { SEARCH_DEBOUNCE_MS } from '../../utilities/defaults.const';
import { paymentActionErrorMessage } from '../payment-action-error.util';

@Component({
	selector: 'app-payments',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, RouterLink, FormsModule, DecimalPipe, DatePipe],
	templateUrl: './payments.component.html'
})
export class PaymentsComponent implements OnInit {
	allPayments: Payment[] = [];
	filteredPayments: Payment[] = [];

	searchString = '';
		statusFilter = '';
		paymentTypeFilter = '';
		fromDateFilter = '';
		toDateFilter = '';
	courseIdFilter = '';
	courseTitleFilter = '';
	studentIdFilter = '';
	studentNameFilter = '';

	showCancelModal = false;
	paymentIdToCancel = '';
	cancelNote = '';

	pageIndex = 1;
	pageSize = 15;
	totalCount = 0;
	totalPages = 0;
	hasPreviousPage = false;
	hasNextPage = false;

	loaderStore = inject(LoaderModalStore);
	noticeStore = inject(NoticeModalStore);
	userStore = inject(UserStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private paymentService = inject(PaymentService);
	private notificationService = inject(NotificationService);
	private activatedRoute = inject(ActivatedRoute);
	private router = inject(Router);
	private searchChanged$ = new Subject<string>();

	private paymentsCueCleared = false;

	constructor() {
		effect(() => {
			if (this.notificationService.unreadPaymentCount() > 0 && !this.paymentsCueCleared) {
				this.paymentsCueCleared = true;
				this.notificationService.markTypeRead('payment');
			}
		});
	}

	ngOnInit() {
			this.searchChanged$
				.pipe(
					debounceTime(SEARCH_DEBOUNCE_MS),
					distinctUntilChanged(),
					takeUntilDestroyed(this.destroyRef)
				)
				.subscribe(() => {
					this.pageIndex = 1;
					this.updateQueryParams();
				});

			this.activatedRoute.queryParams.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {
				this.searchString = params['search'] || '';
				this.courseIdFilter = params['courseId'] || '';
				this.studentIdFilter = params['studentId'] || '';
				this.statusFilter = params['status'] || '';
				this.paymentTypeFilter = params['paymentType'] || '';
				this.fromDateFilter = params['fromDate'] || '';
				this.toDateFilter = params['toDate'] || '';
				this.pageIndex = this.toPositiveInt(params['pageIndex'], 1);
				this.pageSize = this.toPositiveInt(params['pageSize'], 15);
				this.loadPayments();
			});
		}

		private toPositiveInt(value: unknown, fallback: number): number {
			const parsed = Number(value);
			return Number.isInteger(parsed) && parsed > 0 ? parsed : fallback;
		}

		private updateQueryParams() {
			this.router.navigate([], {
				relativeTo: this.activatedRoute,
				queryParams: {
					search: this.searchString.trim() || null,
					courseId: this.courseIdFilter || null,
					studentId: this.studentIdFilter || null,
					status: this.statusFilter || null,
					paymentType: this.paymentTypeFilter || null,
					fromDate: this.fromDateFilter || null,
					toDate: this.toDateFilter || null,
					pageIndex: this.pageIndex > 1 ? this.pageIndex : null,
					pageSize: this.pageSize !== 15 ? this.pageSize : null
				},
				queryParamsHandling: 'merge'
			});
		}

	private updateContextLabels() {
		const coursePayment = this.allPayments.find(p => p.courseId === this.courseIdFilter);
		this.courseTitleFilter = coursePayment?.courseTitle ?? this.courseTitleFilter;
		const studentPayment = this.allPayments.find(p => p.studentId === this.studentIdFilter);
		this.studentNameFilter = studentPayment?.fullName ?? this.studentNameFilter;
	}

	clearContextFilter() {
		this.courseIdFilter = '';
		this.courseTitleFilter = '';
		this.studentIdFilter = '';
		this.studentNameFilter = '';
		this.router.navigate([], { relativeTo: this.activatedRoute, queryParams: {} });
	}

	loadPayments() {
		patchState(this.loaderStore, { isShow: true });
			this.paymentService
				.searchPaymentsPaged(
					this.searchString,
					this.pageIndex,
					this.pageSize,
					this.courseIdFilter,
					this.studentIdFilter,
					this.statusFilter,
					this.paymentTypeFilter,
					this.fromDateFilter,
					this.toDateFilter
				)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: result => {
					this.allPayments = result.items;
					this.filteredPayments = result.items;
					this.totalCount = result.totalCount;
					this.totalPages = result.totalPages;
					this.hasPreviousPage = result.hasPreviousPage;
					this.hasNextPage = result.hasNextPage;
					this.updateContextLabels();
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load payments', 'error');
				}
			});
	}

	onSearch() {
		this.searchChanged$.next(this.searchString);
	}

		onStatusFilter(status: string) {
			this.statusFilter = status;
			this.pageIndex = 1;
			this.updateQueryParams();
		}

		onFilterChange() {
			this.pageIndex = 1;
			this.updateQueryParams();
		}

		clearFilters() {
				this.searchString = '';
				this.statusFilter = '';
				this.paymentTypeFilter = '';
				this.fromDateFilter = '';
			this.toDateFilter = '';
			this.pageIndex = 1;
				this.updateQueryParams();
			}

			statusLabel(status: Payment['paymentStatus'] | undefined): string {
				switch (status) {
					case 'Paid':
						return 'Approved';
					case 'Pending':
					case 'Not Started':
						return 'Pending';
					case 'Expired':
						return 'Expired';
					case 'Canceled':
						return 'Canceled';
					default:
						return status || 'Unknown';
				}
			}

			statusHelp(status: Payment['paymentStatus'] | undefined): string {
				switch (status) {
					case 'Expired':
						return 'Payment window expired. Seat released.';
					case 'Canceled':
						return 'Payment was canceled. Seat released unless another payment is paid.';
					case 'Paid':
						return 'Payment confirmed. Student has course access.';
					default:
						return 'Waiting for payment confirmation.';
				}
			}

			statusBadgeClass(status: Payment['paymentStatus'] | undefined): string {
				const base = 'inline-flex items-center px-2.5 py-0.5 rounded-full text-[9px] font-extrabold uppercase tracking-wide';
				switch (status) {
					case 'Paid':
						return `${base} bg-emerald-100 text-emerald-800 dark:bg-emerald-950/30 dark:text-emerald-400`;
					case 'Pending':
					case 'Not Started':
						return `${base} bg-amber-100 text-amber-800 dark:bg-amber-950/30 dark:text-amber-400`;
					case 'Expired':
						return `${base} bg-slate-100 text-slate-700 dark:bg-slate-800/50 dark:text-slate-300`;
					default:
						return `${base} bg-red-100 text-red-800 dark:bg-red-950/30 dark:text-red-400`;
				}
			}

			goToPage(pageIndex: number) {
				if (pageIndex < 1 || (this.totalPages && pageIndex > this.totalPages) || pageIndex === this.pageIndex) return;
				this.pageIndex = pageIndex;
			this.updateQueryParams();
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
					this.loadPayments();
				},
				error: err => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show(paymentActionErrorMessage(err, 'Failed to approve payment'), 'error');
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
					this.loadPayments();
				},
				error: err => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show(paymentActionErrorMessage(err, 'Failed to cancel payment'), 'error');
				}
			});
	}
}
