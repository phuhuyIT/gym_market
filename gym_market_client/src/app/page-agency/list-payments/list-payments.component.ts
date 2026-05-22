import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { PaymentService } from '../payment.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { NoticeModalStore } from '../../stores/notice.store';
import { patchState } from '@ngrx/signals';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CancelPaymentDto, Payment } from '../../core/models/payment.model';

@Component({
    selector: 'app-list-payments',
    imports: [RouterLink, DatePipe, FormsModule, DecimalPipe],
    templateUrl: './list-payments.component.html',
    styleUrl: './list-payments.component.scss'
})
export class ListPaymentsComponent implements OnInit {
	payments: Payment[] = [];
	notice = inject(NoticeModalStore);
	private destroyRef = inject(DestroyRef);
	private paymentService = inject(PaymentService);
	private activatedRoute = inject(ActivatedRoute);

	showCancel: boolean = false;
	paymentNote: string = '';
	paymentId: string | null = null;

	ngOnInit() {
		this.getPayments();
	}

	private getPayments() {
		this.activatedRoute.params.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (params) => {
				const courseId = params['courseId'];
				if (courseId) {
					this.paymentService.getPayments(courseId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
						next: (res: Payment[]) => {
							this.payments = res;
						},
					});
				}
			},
		});
	}

	okPayment(paymentId: string) {
		this.paymentService.okPayment(paymentId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: () => {
				patchState(this.notice, { message: 'Successfully', isShow: true });
				const pay = this.payments.find((c) => c.paymentId === paymentId);

				if (pay) {
					pay.paymentStatus = 'Paid';
				}
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
