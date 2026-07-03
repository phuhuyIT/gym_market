import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import {
	AssignmentSubmission,
	CourseAssignment,
	SubmitAssignment,
	UpsertCourseAssignment,
} from '../models/assignment.model';

@Injectable({
	providedIn: 'root',
})
export class AssignmentService {
	constructor(private http: HttpClient) {}

	getForManagement(courseId: string): Observable<CourseAssignment[]> {
		return this.http.get<CourseAssignment[]>(`${environment.baseApi}/assignments/course/${courseId}/manage`);
	}

	getForStudent(courseId: string): Observable<CourseAssignment[]> {
		return this.http.get<CourseAssignment[]>(`${environment.baseApi}/assignments/course/${courseId}`);
	}

	create(courseId: string, model: UpsertCourseAssignment): Observable<CourseAssignment> {
		return this.http.post<CourseAssignment>(`${environment.baseApi}/assignments/course/${courseId}`, model);
	}

	update(assignmentId: string, model: UpsertCourseAssignment): Observable<CourseAssignment> {
		return this.http.put<CourseAssignment>(`${environment.baseApi}/assignments/${assignmentId}`, model);
	}

	remove(assignmentId: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/assignments/${assignmentId}`);
	}

	getSubmissions(assignmentId: string): Observable<AssignmentSubmission[]> {
		return this.http.get<AssignmentSubmission[]>(`${environment.baseApi}/assignments/${assignmentId}/submissions`);
	}

	submit(assignmentId: string, model: SubmitAssignment): Observable<AssignmentSubmission> {
		return this.http.post<AssignmentSubmission>(`${environment.baseApi}/assignments/${assignmentId}/submit`, model);
	}

	grade(submissionId: string, score: number, feedback?: string | null): Observable<AssignmentSubmission> {
		return this.http.put<AssignmentSubmission>(`${environment.baseApi}/assignments/submissions/${submissionId}/grade`, {
			score,
			feedback: feedback || null,
		});
	}
}
