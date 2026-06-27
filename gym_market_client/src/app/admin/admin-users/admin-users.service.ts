import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { ApiResponse } from '../../core/models/auth.model';
import { PagedResult } from '../../core/models/paged-result.model';
import { AdminUser, AdminUserDetail } from './admin-user.model';

@Injectable({
	providedIn: 'root',
})
export class AdminUsersService {
	constructor(private http: HttpClient) {}

	searchUsers(params: {
		search?: string;
			role?: string;
			status?: string;
			trainerApprovalStatus?: string;
			emailConfirmed?: string;
			pageIndex?: number;
			pageSize?: number;
	}): Observable<PagedResult<AdminUser>> {
		const query: Record<string, string> = {
			pageIndex: String(params.pageIndex ?? 1),
			pageSize: String(params.pageSize ?? 10),
		};

		if (params.search?.trim()) query['search'] = params.search.trim();
			if (params.role) query['role'] = params.role;
			if (params.status) query['status'] = params.status;
			if (params.trainerApprovalStatus) query['trainerApprovalStatus'] = params.trainerApprovalStatus;
			if (params.emailConfirmed) query['emailConfirmed'] = params.emailConfirmed;

		return this.http.get<PagedResult<AdminUser>>(`${environment.baseApi}/Admin/users`, { params: query });
	}

	getUser(id: string): Observable<AdminUserDetail> {
		return this.http.get<AdminUserDetail>(`${environment.baseApi}/Admin/users/${id}`);
	}

		updateStatus(id: string, status: string): Observable<ApiResponse> {
			return this.http.put<ApiResponse>(`${environment.baseApi}/Admin/users/${id}/status`, { status });
		}

		updateTrainerApproval(id: string, status: string): Observable<ApiResponse> {
			return this.http.put<ApiResponse>(`${environment.baseApi}/Admin/users/${id}/trainer-approval`, { status });
		}

		resendConfirmation(id: string): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${environment.baseApi}/Admin/users/${id}/resend-confirmation`, {});
	}

	unlock(id: string): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${environment.baseApi}/Admin/users/${id}/unlock`, {});
	}
}
