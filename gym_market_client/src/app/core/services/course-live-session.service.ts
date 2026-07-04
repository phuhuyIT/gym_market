import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { CourseCalendarItem, CourseLiveSession, UpsertCourseLiveSession } from '../models/course-live-session.model';

@Injectable({
	providedIn: 'root',
})
export class CourseLiveSessionService {
	constructor(private http: HttpClient) {}

	getForStudent(courseId: string): Observable<CourseLiveSession[]> {
		return this.http.get<CourseLiveSession[]>(`${environment.baseApi}/CourseLiveSessions/course/${courseId}`);
	}

	getForManagement(courseId: string): Observable<CourseLiveSession[]> {
		return this.http.get<CourseLiveSession[]>(`${environment.baseApi}/CourseLiveSessions/course/${courseId}/manage`);
	}

	create(courseId: string, model: UpsertCourseLiveSession): Observable<CourseLiveSession> {
		return this.http.post<CourseLiveSession>(`${environment.baseApi}/CourseLiveSessions/course/${courseId}`, model);
	}

	update(liveSessionId: string, model: UpsertCourseLiveSession): Observable<CourseLiveSession> {
		return this.http.put<CourseLiveSession>(`${environment.baseApi}/CourseLiveSessions/${liveSessionId}`, model);
	}

	remove(liveSessionId: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/CourseLiveSessions/${liveSessionId}`);
	}

	getCalendar(courseId: string, from?: Date, to?: Date): Observable<CourseCalendarItem[]> {
		let params = new HttpParams();
		if (from) params = params.set('from', from.toISOString());
		if (to) params = params.set('to', to.toISOString());
		return this.http.get<CourseCalendarItem[]>(`${environment.baseApi}/CourseCalendar/course/${courseId}`, { params });
	}
}
