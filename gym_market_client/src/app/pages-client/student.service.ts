import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Student, StudentProfileResponse, UpdateStudentProfileDto } from '../core/models/student.model';

@Injectable({
	providedIn: 'root',
})
export class StudentService {
	constructor(private http: HttpClient) {}

	getStudentInfo(studentId: string): Observable<Student> {
		return this.http.get<Student>(`${environment.baseApi}/student/${studentId}`);
	}

	// Both endpoints only ever serve the authenticated user's own record (the
	// backend resolves the user from the JWT), so no userId is sent.
	getOwnStudentInfo(): Observable<Student> {
		return this.http.get<Student>(`${environment.baseApi}/student/by-user`);
	}

	getOwnStudentProfile(): Observable<StudentProfileResponse> {
		return this.http.get<StudentProfileResponse>(`${environment.baseApi}/student/profile`);
	}

	updateStudentProfile(model: UpdateStudentProfileDto, studentId: string): Observable<void> {
		return this.http.put<void>(`${environment.baseApi}/student/${studentId}`, model);
	}
}
