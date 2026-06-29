import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { ClassBooking, ClassSession, UpsertClassSession } from '../models/class-schedule.model';

@Injectable({
	providedIn: 'root',
})
export class ClassScheduleService {
	constructor(private http: HttpClient) {}

	getSessions(from?: Date, to?: Date, includeCancelled = false): Observable<ClassSession[]> {
		let params = new HttpParams().set('includeCancelled', includeCancelled);
		if (from) params = params.set('from', from.toISOString());
		if (to) params = params.set('to', to.toISOString());
		return this.http.get<ClassSession[]>(`${environment.baseApi}/ClassSchedule/sessions`, { params });
	}

	getManageSessions(includeCancelled = true): Observable<ClassSession[]> {
		const params = new HttpParams().set('includeCancelled', includeCancelled);
		return this.http.get<ClassSession[]>(`${environment.baseApi}/ClassSchedule/manage/sessions`, { params });
	}

	createSession(model: UpsertClassSession): Observable<ClassSession> {
		return this.http.post<ClassSession>(`${environment.baseApi}/ClassSchedule/sessions`, model);
	}

	updateSession(classSessionId: string, model: UpsertClassSession): Observable<ClassSession> {
		return this.http.put<ClassSession>(`${environment.baseApi}/ClassSchedule/sessions/${classSessionId}`, model);
	}

	cancelSession(classSessionId: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/ClassSchedule/sessions/${classSessionId}`);
	}

	getBookings(classSessionId: string): Observable<ClassBooking[]> {
		return this.http.get<ClassBooking[]>(`${environment.baseApi}/ClassSchedule/sessions/${classSessionId}/bookings`);
	}

	bookSession(classSessionId: string): Observable<ClassBooking> {
		return this.http.post<ClassBooking>(`${environment.baseApi}/ClassSchedule/sessions/${classSessionId}/book`, {});
	}

	cancelBooking(bookingId: string): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/ClassSchedule/bookings/${bookingId}/cancel`, {});
	}

	markAttendance(bookingId: string, status: 'Attended' | 'NoShow'): Observable<ClassBooking> {
		return this.http.post<ClassBooking>(`${environment.baseApi}/ClassSchedule/bookings/${bookingId}/attendance`, { status });
	}
}
