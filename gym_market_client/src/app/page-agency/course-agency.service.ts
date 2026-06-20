import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Course } from '../core/models/course.model';
import { PagedResult } from '../core/models/paged-result.model';

@Injectable({
	providedIn: 'root',
})
export class CourseAgencyService {
	searchString = signal<string>('');

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
		return this.getCoursesPaged(pageIndex, pageSize, searchString, category).pipe(
			map(result => result.items)
		);
	}

	getCoursesPaged(
		pageIndex: number,
		pageSize: number,
		searchString: string | null,
		category: string | null
	): Observable<PagedResult<Course>> {
		return this.http.get<PagedResult<Course>>(`${environment.baseApi}/course/get-courses`, {
			params: {
				pageIndex: pageIndex.toString(),
				pageSize: pageSize.toString(),
				searchString: searchString ?? '',
				category: category ?? '',
			},
		});
	}

	getCourse(id: string): Observable<Course> {
		return this.http.get<Course>(`${environment.baseApi}/course/get-course/${id}`);
	}

	updateCourse(model: Partial<Course> | FormData): Observable<void> {
		return this.http.put<void>(`${environment.baseApi}/course/update-course/`, model);
	}

	removeCourse(id: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/course/${id}`);
	}

	getCoursesOfTrainer(trainerId: string): Observable<Course[]> {
		return this.http.get<Course[]>(
			`${environment.baseApi}/course/get-courses-of-trainer/${trainerId}`
		);
	}
}
