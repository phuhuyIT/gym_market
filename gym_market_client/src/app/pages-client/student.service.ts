import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Student, UpdateStudentProfileDto } from '../core/models/student.model';

@Injectable({
	providedIn: 'root',
})
export class StudentService {
	constructor(private http: HttpClient) {}

	getStudentInfo(studentId: string): Observable<Student> {
		return this.http.get<Student>(`${environment.baseApi}/student/${studentId}`);
	}

	updateStudentProfile(model: UpdateStudentProfileDto, studentId: string): Observable<any> {
		return this.http.put(`${environment.baseApi}/student/${studentId}`, model);
	}
}
