import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { ProgressGoal, ProgressLog, ProgressSummary, UpsertProgressGoal, UpsertProgressLog } from '../models/progress.model';

@Injectable({
	providedIn: 'root',
})
export class ProgressService {
	constructor(private http: HttpClient) {}

	getMyLogs(): Observable<ProgressLog[]> {
		return this.http.get<ProgressLog[]>(`${environment.baseApi}/Progress/me/logs`);
	}

	createMyLog(model: UpsertProgressLog): Observable<ProgressLog> {
		return this.http.post<ProgressLog>(`${environment.baseApi}/Progress/me/logs`, model);
	}

	getMyGoal(): Observable<ProgressGoal | null> {
		return this.http.get<ProgressGoal | null>(`${environment.baseApi}/Progress/me/goal`);
	}

	saveMyGoal(model: UpsertProgressGoal): Observable<ProgressGoal> {
		return this.http.put<ProgressGoal>(`${environment.baseApi}/Progress/me/goal`, model);
	}

	getSummaries(): Observable<ProgressSummary[]> {
		return this.http.get<ProgressSummary[]>(`${environment.baseApi}/Progress/summaries`);
	}

	getStudentLogs(studentId: string): Observable<ProgressLog[]> {
		return this.http.get<ProgressLog[]>(`${environment.baseApi}/Progress/students/${studentId}/logs`);
	}

	getStudentGoal(studentId: string): Observable<ProgressGoal | null> {
		return this.http.get<ProgressGoal | null>(`${environment.baseApi}/Progress/students/${studentId}/goal`);
	}
}
