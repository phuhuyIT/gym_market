import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import {
	MembershipPlan,
	MembershipStatus,
	StudentMembership,
	UpsertMembershipPlan,
} from '../models/membership.model';

@Injectable({
	providedIn: 'root',
})
export class MembershipService {
	constructor(private http: HttpClient) {}

	getPlans(includeInactive = false): Observable<MembershipPlan[]> {
		const params = new HttpParams().set('includeInactive', includeInactive);
		return this.http.get<MembershipPlan[]>(`${environment.baseApi}/Memberships/plans`, { params });
	}

	createPlan(model: UpsertMembershipPlan): Observable<MembershipPlan> {
		return this.http.post<MembershipPlan>(`${environment.baseApi}/Memberships/plans`, model);
	}

	updatePlan(planId: string, model: UpsertMembershipPlan): Observable<MembershipPlan> {
		return this.http.put<MembershipPlan>(`${environment.baseApi}/Memberships/plans/${planId}`, model);
	}

	deactivatePlan(planId: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/Memberships/plans/${planId}`);
	}

	getMyStatus(): Observable<MembershipStatus> {
		return this.http.get<MembershipStatus>(`${environment.baseApi}/Memberships/me/status`);
	}

	subscribe(planId: string): Observable<StudentMembership> {
		return this.http.post<StudentMembership>(`${environment.baseApi}/Memberships/subscribe`, { planId });
	}

	cancelMine(): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/Memberships/me/cancel`, {});
	}

	getSubscriptions(status = ''): Observable<StudentMembership[]> {
		let params = new HttpParams();
		if (status) params = params.set('status', status);
		return this.http.get<StudentMembership[]>(`${environment.baseApi}/Memberships/subscriptions`, { params });
	}
}
