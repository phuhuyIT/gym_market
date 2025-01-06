import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';

@Injectable({
	providedIn: 'root',
})
export class PaymentService {
	constructor(private http: HttpClient) {}

	getPayments(courseId: string) {
		return this.http.get(`${environment.baseApi}/Payments/get-payments-ofcourse/${courseId}`);
	}

	okPayment(paymentId: string) {
		return this.http.post(`${environment.baseApi}/Payments/ok-payment/${paymentId}`, {});
	}

    cancelPayment(model: any) {
		return this.http.post(`${environment.baseApi}/Payments/cancel-payment`, model);
	}
}
