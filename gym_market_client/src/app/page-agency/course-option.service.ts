import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { CourseOption } from '../core/models/course.model';

@Injectable({
	providedIn: 'root',
})
export class CourseOptionService {
	constructor(private http: HttpClient) {}

	getAllCourseOptions(): Observable<CourseOption[]> {
		return this.http.get<CourseOption[]>(`${environment.baseApi}/courseoption`);
	}

	getCourseOptionsByCourseId(courseId: string): Observable<CourseOption[]> {
		return this.http.get<CourseOption[]>(
			`${environment.baseApi}/courseoption/course/${courseId}`
		);
	}

	addCourseOptionOftrainer(model: CourseOption): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/courseoption`, model);
	}

	removeCourseOptionOftrainer(id: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/courseoption/${id}`);
	}

	updateCourseOptionOftrainer(model: CourseOption): Observable<void> {
		return this.http.put<void>(`${environment.baseApi}/courseoption/${model.optionId}`, model);
	}
}
