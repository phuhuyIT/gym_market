import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Lecture, LectureMaterial } from '../core/models/lecture.model';

@Injectable({
	providedIn: 'root',
})
export class CourseMaterialService {
	constructor(private http: HttpClient) {}

	// ----- Lectures -----
	getLecturesByCourse(courseId: string): Observable<Lecture[]> {
		return this.http.get<Lecture[]>(`${environment.baseApi}/lecture/course/${courseId}`);
	}

	addLecture(model: Lecture): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/lecture`, model);
	}

	updateLecture(model: Lecture): Observable<void> {
		return this.http.put<void>(`${environment.baseApi}/lecture/${model.lectureId}`, model);
	}

	removeLecture(lectureId: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/lecture/${lectureId}`);
	}

	// ----- Lecture materials -----
	getMaterialsByLecture(lectureId: string): Observable<LectureMaterial[]> {
		return this.http.get<LectureMaterial[]>(
			`${environment.baseApi}/lecturematerial/lecture/${lectureId}`
		);
	}

	addMaterial(model: LectureMaterial): Observable<void> {
		return this.http.post<void>(`${environment.baseApi}/lecturematerial`, model);
	}

	updateMaterial(model: LectureMaterial): Observable<void> {
		return this.http.put<void>(
			`${environment.baseApi}/lecturematerial/${model.materialId}`,
			model
		);
	}

	removeMaterial(materialId: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/lecturematerial/${materialId}`);
	}
}
