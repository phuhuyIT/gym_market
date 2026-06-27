import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { ApiResponse } from '../../core/models/auth.model';
import { PagedResult } from '../../core/models/paged-result.model';
import { AdminCourse, CourseModerationStatus } from './admin-course.model';

@Injectable({
	providedIn: 'root',
})
export class AdminCoursesService {
	constructor(private http: HttpClient) {}

	searchCourses(params: {
		search?: string;
		status?: string;
		pageIndex?: number;
		pageSize?: number;
	}): Observable<PagedResult<AdminCourse>> {
		const query: Record<string, string> = {
			pageIndex: String(params.pageIndex ?? 1),
			pageSize: String(params.pageSize ?? 10),
		};

		if (params.search?.trim()) query['search'] = params.search.trim();
		if (params.status) query['status'] = params.status;

		return this.http.get<PagedResult<AdminCourse>>(`${environment.baseApi}/Course/admin-review`, { params: query });
	}

	updateModeration(courseId: string, status: CourseModerationStatus): Observable<ApiResponse> {
		return this.http.put<ApiResponse>(`${environment.baseApi}/Course/${courseId}/moderation`, { status });
	}
}
