import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { CourseAnalyticsDashboard, MyCourseAnalytics } from '../models/course-analytics.model';

@Injectable({
	providedIn: 'root',
})
export class CourseAnalyticsService {
	constructor(private http: HttpClient) {}

	getCourseDashboard(courseId: string): Observable<CourseAnalyticsDashboard> {
		return this.http.get<CourseAnalyticsDashboard>(`${environment.baseApi}/courseanalytics/course/${courseId}`);
	}

	getMyCourseProgress(courseId: string): Observable<MyCourseAnalytics> {
		return this.http.get<MyCourseAnalytics>(`${environment.baseApi}/courseanalytics/course/${courseId}/me`);
	}
}
