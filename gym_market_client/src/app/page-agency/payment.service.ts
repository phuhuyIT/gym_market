import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CancelPaymentDto, Payment } from '../core/models/payment.model';
import { PagedResult } from '../core/models/paged-result.model';

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

	searchPaymentsPaged(
		search = '',
		pageIndex = 1,
			pageSize = 15,
			courseId = '',
			studentId = '',
			status = '',
			paymentType = '',
			fromDate = '',
			toDate = ''
		): Observable<PagedResult<Payment>> {
		let params = new HttpParams()
			.set('search', search.trim())
			.set('pageIndex', pageIndex)
			.set('pageSize', pageSize);

			if (courseId) params = params.set('courseId', courseId);
			if (studentId) params = params.set('studentId', studentId);
			if (status) params = params.set('status', status);
			if (paymentType) params = params.set('paymentType', paymentType);
			if (fromDate) params = params.set('fromDate', fromDate);
			if (toDate) params = params.set('toDate', toDate);

			return this.http.get<PagedResult<Payment>>(`${environment.baseApi}/Payments/search`, { params });
		}

	okPayment(paymentId: string): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/Payments/ok-payment/${paymentId}`, {});
	}

	cancelPayment(model: CancelPaymentDto): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/Payments/cancel-payment`, model);
	}
}
