import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';

@Injectable({
	providedIn: 'root',
})
export class CouresRegistrationService {
	constructor(private http: HttpClient) {}

	registerCourse(courseId: string, studentId: string) {
		return this.http.post(`${environment.baseApi}/CourseRegistration/register-course`, {
			courseId,
			studentId,
		});
	}

	getCourses(studentId: string) {
        return this.http.get(`${environment.baseApi}/CourseRegistration/get-course-registrations/${studentId}`);
    }

	cancelCourse(courseId: string, studentId: string) {}
}
