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

	getStudentInfoByUserId(userId: string): Observable<Student> {
		return this.http.get<Student>(`${environment.baseApi}/student/by-user/${userId}`);
	}

	getStudentProfile(userId: string): Observable<StudentProfileResponse> {
		return this.http.get<StudentProfileResponse>(`${environment.baseApi}/student/profile/${userId}`);
	}

	updateStudentProfile(model: UpdateStudentProfileDto, studentId: string): Observable<void> {
		return this.http.put<void>(`${environment.baseApi}/student/${studentId}`, model);
	}
}
