import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { UpdateStudentProfileDto } from './models/update-student-profile.dto';

@Injectable({
	providedIn: 'root',
})
export class StudentService {
	constructor(private http: HttpClient) {}

	getStudentInfo(studentId: string) {
		return this.http.get(`${environment.baseApi}/student/${studentId}`);
	}

	updateStudentProfile(model: UpdateStudentProfileDto, studentId: string) {
		return this.http.put(`${environment.baseApi}/student/${studentId}`, model);
	}
}
