import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import {
	CourseGradebook,
	GradebookPolicy,
	MyCourseGrades,
	UpdateGradebookPolicy,
} from '../models/gradebook.model';

@Injectable({
	providedIn: 'root',
})
export class GradebookService {
	constructor(private http: HttpClient) {}

	getPolicy(courseId: string): Observable<GradebookPolicy> {
		return this.http.get<GradebookPolicy>(`${environment.baseApi}/gradebook/course/${courseId}/policy`);
	}

	updatePolicy(courseId: string, model: UpdateGradebookPolicy): Observable<GradebookPolicy> {
		return this.http.put<GradebookPolicy>(`${environment.baseApi}/gradebook/course/${courseId}/policy`, model);
	}

	getCourseGradebook(courseId: string): Observable<CourseGradebook> {
		return this.http.get<CourseGradebook>(`${environment.baseApi}/gradebook/course/${courseId}`);
	}

	getMyGrades(courseId: string): Observable<MyCourseGrades> {
		return this.http.get<MyCourseGrades>(`${environment.baseApi}/gradebook/course/${courseId}/me`);
	}

	exportCourseGradebook(courseId: string): Observable<Blob> {
		return this.http.get(`${environment.baseApi}/gradebook/course/${courseId}/export`, {
			responseType: 'blob',
		});
	}
}
