import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';

@Injectable({
	providedIn: 'root',
})
export class CourseAgencyService {
	constructor(private http: HttpClient) {}

	addCourse(model: any) {
		return this.http.post(`${environment.baseApi}/course`, model);
	}

	getCourses(
		pageIndex: number,
		pageSize: number,
		searchString: string | null,
		category: string | null
	) {
		return this.http.get(
			`${environment.baseApi}/course/get-courses?pageIndex=${pageIndex}&pageSize=${pageSize}&searchString=${searchString}&category=${category}`
		);
	}

	getCourse(id: string) {
		return this.http.get(`${environment.baseApi}/Course/get-course/${id}`);
	}

	updateCourse(model: any) {
		return this.http.put(`${environment.baseApi}/Course/update-course/`, model);
	}

	removeCourse(id: string) {
		return this.http.delete(`${environment.baseApi}/course/${id}`);
	}

	getCoursesOftrainer(trainerId: string) {
		return this.http.get(`${environment.baseApi}/Course/get-courses-of-trainer/${trainerId}`);
	}
}
