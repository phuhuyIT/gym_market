import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';

@Injectable({
	providedIn: 'root',
})
export class CourseRatingService {
	constructor(private http: HttpClient) {}

	addRating(model: any) {
		return this.http.post(`${environment.baseApi}/CourseRating/add-course-rating`, model);
	}

	getCourseRatings(courseId: string) {
		return this.http.get(`${environment.baseApi}/CourseRating/get-course-rating/${courseId}`);
	}
}
