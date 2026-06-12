import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Course } from '../core/models/course.model';

@Injectable({
	providedIn: 'root',
})
export class CourseRegistrationService {
	constructor(private http: HttpClient) {}

	// The acting student is derived from the JWT on the backend, so no
	// studentId is ever sent.
	registerCourse(courseId: string): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/CourseRegistration/register-course`, {
			courseId,
		});
	}

	// The API returns the registered courses (with statusPayment), not registration rows.
	getCourses(): Observable<Course[]> {
		return this.http.get<Course[]>(
			`${environment.baseApi}/CourseRegistration/get-course-registrations`
		);
	}

	cancelRegistration(registrationId: string): Observable<void> {
		return this.http.post<void>(
			`${environment.baseApi}/CourseRegistration/cancel-registration/${registrationId}`,
			{}
		);
	}
}
