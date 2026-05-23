import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CourseRegistration } from '../core/models/course-registration.model';

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

	getCourses(studentId: string): Observable<CourseRegistration[]> {
		return this.http.get<CourseRegistration[]>(
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
