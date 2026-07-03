import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { CourseAnnouncement, UpsertCourseAnnouncement } from '../models/announcement.model';

@Injectable({
	providedIn: 'root',
})
export class AnnouncementService {
	constructor(private http: HttpClient) {}

	getCourseAnnouncements(courseId: string): Observable<CourseAnnouncement[]> {
		return this.http.get<CourseAnnouncement[]>(`${environment.baseApi}/CourseAnnouncements/course/${courseId}`);
	}

	getAnnouncement(announcementId: string): Observable<CourseAnnouncement> {
		return this.http.get<CourseAnnouncement>(`${environment.baseApi}/CourseAnnouncements/${announcementId}`);
	}

	createAnnouncement(courseId: string, model: UpsertCourseAnnouncement): Observable<CourseAnnouncement> {
		return this.http.post<CourseAnnouncement>(`${environment.baseApi}/CourseAnnouncements/course/${courseId}`, model);
	}

	updateAnnouncement(announcementId: string, model: UpsertCourseAnnouncement): Observable<CourseAnnouncement> {
		return this.http.put<CourseAnnouncement>(`${environment.baseApi}/CourseAnnouncements/${announcementId}`, model);
	}

	deleteAnnouncement(announcementId: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/CourseAnnouncements/${announcementId}`);
	}
}
