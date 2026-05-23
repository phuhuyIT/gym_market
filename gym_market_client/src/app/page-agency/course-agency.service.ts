import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Course } from '../core/models/course.model';

@Injectable({
	providedIn: 'root',
})
export class CourseAgencyService {
	constructor(private http: HttpClient) {}

	addCourse(model: Partial<Course>): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/course`, model);
	}

	getCourses(
		pageIndex: number,
		pageSize: number,
		searchString: string | null,
		category: string | null
	): Observable<Course[]> {
		return this.http.get<Course[]>(`${environment.baseApi}/course/get-courses`, {
			params: {
				pageIndex: pageIndex.toString(),
				pageSize: pageSize.toString(),
				searchString: searchString ?? '',
				category: category ?? '',
			},
		});
	}

	getCourse(id: string): Observable<Course> {
		return this.http.get<Course>(`${environment.baseApi}/Course/get-course/${id}`);
	}

	updateCourse(model: Partial<Course>): Observable<void> {
		return this.http.put<void>(`${environment.baseApi}/Course/update-course/`, model);
	}

	removeCourse(id: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/course/${id}`);
	}

	getCoursesOftrainer(trainerId: string): Observable<Course[]> {
		return this.http.get<Course[]>(
			`${environment.baseApi}/Course/get-courses-of-trainer/${trainerId}`
		);
	}
}
