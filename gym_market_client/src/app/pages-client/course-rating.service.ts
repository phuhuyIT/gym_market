import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { CourseRating, CourseRatingCreateDto } from '../core/models/course.model';

@Injectable({
	providedIn: 'root',
})
export class CourseRatingService {
	constructor(private http: HttpClient) {}

	addRating(model: CourseRatingCreateDto): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/CourseRating/add-course-rating`, model);
	}

	getCourseRatings(courseId: string): Observable<CourseRating[]> {
		return this.http.get<CourseRating[]>(
			`${environment.baseApi}/CourseRating/get-course-rating/${courseId}`
		);
	}
}
