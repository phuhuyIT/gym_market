import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CancelPaymentDto, Payment } from '../core/models/payment.model';

@Injectable({
	providedIn: 'root',
})
export class PaymentService {
	constructor(private http: HttpClient) {}

	getPayments(courseId: string): Observable<Payment[]> {
		return this.http.get<Payment[]>(
			`${environment.baseApi}/Payments/get-payments-of-course/${courseId}`
		);
	}

	okPayment(paymentId: string): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/Payments/ok-payment/${paymentId}`, {});
	}

	cancelPayment(model: CancelPaymentDto): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/Payments/cancel-payment`, model);
	}
}
