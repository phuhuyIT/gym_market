import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { AddCourseOption } from './models/add-course-option.model';

@Injectable({
	providedIn: 'root',
})
export class CourseOptionService {
	constructor(private http: HttpClient) {}

	getCourseOptionsOftrainer() {
		return this.http.get(`${environment.baseApi}/courseoption`);
	}

	addCourseOptionOftrainer(model: AddCourseOption) {
		return this.http.post(`${environment.baseApi}/courseoption`, model);
	}

	removeCourseOptionOftrainer(id: string) {
		return this.http.delete(`${environment.baseApi}/courseoption/${id}`);
	}

    updateCourseOptionOftrainer(model: AddCourseOption) {
        
        return this.http.put(`${environment.baseApi}/courseoption/${model.optionId}`, model);
    }
}
