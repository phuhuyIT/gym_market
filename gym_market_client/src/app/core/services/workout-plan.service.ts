import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { StudentWorkoutAssignment, UpsertWorkoutPlan, WorkoutPlan } from '../models/workout-plan.model';

@Injectable({
	providedIn: 'root',
})
export class WorkoutPlanService {
	constructor(private http: HttpClient) {}

	getPlans(includeInactive = false): Observable<WorkoutPlan[]> {
		const params = new HttpParams().set('includeInactive', includeInactive);
		return this.http.get<WorkoutPlan[]>(`${environment.baseApi}/WorkoutPlans/plans`, { params });
	}

	createPlan(model: UpsertWorkoutPlan): Observable<WorkoutPlan> {
		return this.http.post<WorkoutPlan>(`${environment.baseApi}/WorkoutPlans/plans`, model);
	}

	updatePlan(workoutPlanId: string, model: UpsertWorkoutPlan): Observable<WorkoutPlan> {
		return this.http.put<WorkoutPlan>(`${environment.baseApi}/WorkoutPlans/plans/${workoutPlanId}`, model);
	}

	deactivatePlan(workoutPlanId: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/WorkoutPlans/plans/${workoutPlanId}`);
	}

	assignPlan(workoutPlanId: string, studentId: string, startsAt?: string | Date | null): Observable<StudentWorkoutAssignment> {
		return this.http.post<StudentWorkoutAssignment>(`${environment.baseApi}/WorkoutPlans/plans/${workoutPlanId}/assign`, {
			studentId,
			startsAt,
		});
	}

	getAssignments(status = ''): Observable<StudentWorkoutAssignment[]> {
		let params = new HttpParams();
		if (status) params = params.set('status', status);
		return this.http.get<StudentWorkoutAssignment[]>(`${environment.baseApi}/WorkoutPlans/assignments`, { params });
	}

	getMyAssignments(status = ''): Observable<StudentWorkoutAssignment[]> {
		let params = new HttpParams();
		if (status) params = params.set('status', status);
		return this.http.get<StudentWorkoutAssignment[]>(`${environment.baseApi}/WorkoutPlans/my-assignments`, { params });
	}

	completeExercise(assignmentId: string, exerciseId: string, notes = ''): Observable<StudentWorkoutAssignment> {
		return this.http.post<StudentWorkoutAssignment>(
			`${environment.baseApi}/WorkoutPlans/assignments/${assignmentId}/exercises/${exerciseId}/complete`,
			{ notes }
		);
	}

	cancelAssignment(assignmentId: string): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/WorkoutPlans/assignments/${assignmentId}/cancel`, {});
	}
}
