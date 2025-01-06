import { Component, inject } from '@angular/core';
import { PaymentService } from '../payment.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { NoticeModalStore } from '../../stores/notice.store';
import { patchState } from '@ngrx/signals';
import { FormsModule } from '@angular/forms';

@Component({
	selector: 'app-list-payments',
	standalone: true,
	imports: [RouterLink, DatePipe, FormsModule],
	templateUrl: './list-payments.component.html',
	styleUrl: './list-payments.component.scss',
})
export class ListPaymentsComponent {
	payments: any = [];
	notice = inject(NoticeModalStore);

	showCancel: boolean = false;
	paymentNote: string = '';
	paymentId: string | null = null;

	constructor(private paymentService: PaymentService, private activatedRoute: ActivatedRoute) {}

	ngOnInit() {
		this.getPayments();
	}

	private getPayments() {
		this.activatedRoute.params.subscribe({
			next: (params: any) => {
				console.log(params.courseId); // {id: '2', name: 'hoc'}

				this.paymentService.getPayments(params.courseId).subscribe({
					next: (res: any) => {
						console.log(res);
						this.payments = res;
					},
				});
			},
		});
	}

	okPayment(paymentId: string) {
		this.paymentService.okPayment(paymentId).subscribe({
			next: (res: any) => {
				patchState(this.notice, { message: 'Successfully', isShow: true });
				const pay = this.payments.find((c: any) => c.paymentId === paymentId);

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
		const model = { paymentId: this.paymentId, note: this.paymentNote };
		this.paymentService.cancelPayment(model).subscribe({
			next: (res: any) => {
				patchState(this.notice, { message: 'Successfully', isShow: true });
				const pay = this.payments.find((c: any) => c.paymentId === this.paymentId);

				if (pay) {
					pay.paymentStatus = 'Canceled';
					pay.paymentNote = this.paymentNote;
				}

				this.paymentNote = '';
				this.paymentId = null;
			},
		});
	}
}
