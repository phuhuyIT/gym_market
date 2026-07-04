import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { ApiResponse } from '../models/auth.model';
import { CourseStudyGroup, EligibleCourseLearner, UpsertCourseStudyGroup } from '../models/course-study-group.model';

@Injectable({
	providedIn: 'root',
})
export class CourseStudyGroupService {
	constructor(private http: HttpClient) {}

	getForStudent(courseId: string): Observable<CourseStudyGroup[]> {
		return this.http.get<CourseStudyGroup[]>(`${environment.baseApi}/CourseStudyGroups/course/${courseId}`);
	}

	getForManagement(courseId: string): Observable<CourseStudyGroup[]> {
		return this.http.get<CourseStudyGroup[]>(`${environment.baseApi}/CourseStudyGroups/course/${courseId}/manage`);
	}

	syncDefaultCohort(courseId: string): Observable<CourseStudyGroup> {
		return this.http.post<CourseStudyGroup>(`${environment.baseApi}/CourseStudyGroups/course/${courseId}/sync`, {});
	}

	getEligibleLearners(courseId: string, studyGroupId?: string): Observable<EligibleCourseLearner[]> {
		let params = new HttpParams();
		if (studyGroupId) params = params.set('studyGroupId', studyGroupId);
		return this.http.get<EligibleCourseLearner[]>(`${environment.baseApi}/CourseStudyGroups/course/${courseId}/eligible-learners`, { params });
	}

	create(courseId: string, model: UpsertCourseStudyGroup): Observable<CourseStudyGroup> {
		return this.http.post<CourseStudyGroup>(`${environment.baseApi}/CourseStudyGroups/course/${courseId}/groups`, model);
	}

	update(studyGroupId: string, model: UpsertCourseStudyGroup): Observable<CourseStudyGroup> {
		return this.http.put<CourseStudyGroup>(`${environment.baseApi}/CourseStudyGroups/groups/${studyGroupId}`, model);
	}

	addMembers(studyGroupId: string, userIds: string[]): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${environment.baseApi}/CourseStudyGroups/groups/${studyGroupId}/members`, { userIds });
	}

	removeMember(studyGroupId: string, userId: string): Observable<ApiResponse> {
		return this.http.delete<ApiResponse>(`${environment.baseApi}/CourseStudyGroups/groups/${studyGroupId}/members/${userId}`);
	}

	updateMemberRole(studyGroupId: string, userId: string, role: 'Admin' | 'Member'): Observable<ApiResponse> {
		return this.http.put<ApiResponse>(`${environment.baseApi}/CourseStudyGroups/groups/${studyGroupId}/members/${userId}/role`, { role });
	}

	archive(studyGroupId: string): Observable<ApiResponse> {
		return this.http.delete<ApiResponse>(`${environment.baseApi}/CourseStudyGroups/groups/${studyGroupId}`);
	}
}
