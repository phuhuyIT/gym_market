import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { PaymentService } from '../payment.service';
import { ActivatedRoute } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { NoticeModalStore } from '../../stores/notice.store';
import { patchState } from '@ngrx/signals';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CancelPaymentDto, Payment } from '../../core/models/payment.model';

@Component({
    selector: 'app-list-payments',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [DatePipe, FormsModule, DecimalPipe],
    templateUrl: './list-payments.component.html',
    styleUrl: './list-payments.component.scss'
})
export class ListPaymentsComponent implements OnInit {
	payments: Payment[] = [];
	notice = inject(NoticeModalStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private paymentService = inject(PaymentService);
	private activatedRoute = inject(ActivatedRoute);

	showCancel: boolean = false;
	paymentNote: string = '';
	paymentId: string | null = null;
	courseId = '';
	searchString = '';
	pageIndex = 1;
	pageSize = 15;
	totalCount = 0;
	totalPages = 0;
	hasPreviousPage = false;
	hasNextPage = false;

	ngOnInit() {
		this.getPayments();
	}

	private getPayments() {
		this.activatedRoute.params.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (params) => {
				this.courseId = params['courseId'];
				if (this.courseId) {
					this.loadPayments();
				}
			},
		});
	}

	loadPayments() {
		this.paymentService
			.searchPaymentsPaged(this.searchString, this.pageIndex, this.pageSize, this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: result => {
					this.payments = result.items;
					this.totalCount = result.totalCount;
					this.totalPages = result.totalPages;
					this.hasPreviousPage = result.hasPreviousPage;
					this.hasNextPage = result.hasNextPage;
					this.cdr.markForCheck();
				},
			});
	}

	onSearch() {
		this.pageIndex = 1;
		this.loadPayments();
	}

	goToPage(pageIndex: number) {
		if (pageIndex < 1 || (this.totalPages && pageIndex > this.totalPages) || pageIndex === this.pageIndex) return;
		this.pageIndex = pageIndex;
		this.loadPayments();
	}

	okPayment(paymentId: string) {
		this.paymentService.okPayment(paymentId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: () => {
				patchState(this.notice, { message: 'Successfully', isShow: true });
				const pay = this.payments.find((c) => c.paymentId === paymentId);

					if (pay) {
						pay.paymentStatus = 'Paid';
					}
					this.loadPayments();
					this.cdr.markForCheck();
			},
		});
	}

	onShowCancelNote(flag: boolean, paymentId: string | null) {
		this.showCancel = flag;
		this.paymentId = paymentId;
	}

	cancelPayment() {
		if (!this.paymentId) return;

		const model: CancelPaymentDto = { 
			paymentId: this.paymentId, 
			note: this.paymentNote 
		};
		
		this.paymentService.cancelPayment(model).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: () => {
				patchState(this.notice, { message: 'Successfully', isShow: true });
				const pay = this.payments.find((c) => c.paymentId === this.paymentId);

				if (pay) {
					pay.paymentStatus = 'Canceled';
					pay.note = this.paymentNote;
				}

				this.paymentNote = '';
					this.paymentId = null;
					this.showCancel = false;
					this.loadPayments();
					this.cdr.markForCheck();
			},
		});
	}

	get showDelete() {
		return this.showCancel;
	}

	onShowDelete(flag: boolean, item: Payment | null) {
		this.onShowCancelNote(flag, item?.paymentId ?? null);
	}

	onDelete() {
		this.cancelPayment();
	}
}
