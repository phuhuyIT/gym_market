import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';

@Injectable({
	providedIn: 'root',
})
export class CourseClientService {
	constructor(private http: HttpClient) {}

    getCourses() {
        return this.http.get(`${environment.baseApi}/Course`);
    }
}
