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

	registerCourse(courseId: string, studentId: string): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/CourseRegistration/register-course`, {
			courseId,
			studentId,
		});
	}

	// The API returns the registered courses (with statusPayment), not registration rows.
	getCourses(studentId: string): Observable<Course[]> {
		return this.http.get<Course[]>(
			`${environment.baseApi}/CourseRegistration/get-course-registrations/${studentId}`
		);
	}

	cancelCourse(courseId: string, studentId: string): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/CourseRegistration/cancel-course`, {
			courseId,
			studentId,
		});
	}
}
