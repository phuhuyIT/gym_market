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

	getCourses() {
		return this.http.get(`${environment.baseApi}/course`);
	}

	getCourse(id: string) {
		return this.http.get(`${environment.baseApi}/course/${id}`);
	}

	updateCourse(model: any) {
		return this.http.put(`${environment.baseApi}/course/${model.courseId}`, model);
	}

	removeCourse(id: string) {
		return this.http.delete(`${environment.baseApi}/course/${id}`);
	}

    
}
