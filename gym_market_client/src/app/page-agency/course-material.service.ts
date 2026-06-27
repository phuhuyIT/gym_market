import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Lecture, LectureMaterial } from '../core/models/lecture.model';
import {
	CourseQuiz,
	QuizAttemptSummary,
	SubmitQuizAttempt,
	TrainerCourseQuiz,
	UpsertCourseQuiz,
} from '../core/models/quiz.model';
import { CourseCertificate, CourseCompletionStatus } from '../core/models/certificate.model';

export interface CourseProgress {
	courseId: string;
	totalLectures: number;
	completedLectures: number;
	progressPercent: number;
	completedLectureIds: string[];
}

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

	// ----- Student progress -----
	getCourseProgress(courseId: string): Observable<CourseProgress> {
		return this.http.get<CourseProgress>(
			`${environment.baseApi}/lectureprogress/course/${courseId}`
		);
	}

	updateLectureProgress(lectureId: string, isCompleted: boolean): Observable<void> {
		return this.http.put<void>(
			`${environment.baseApi}/lectureprogress/lecture/${lectureId}`,
			{ isCompleted }
		);
	}

	// ----- Course quiz -----
	getManageQuiz(courseId: string): Observable<TrainerCourseQuiz> {
		return this.http.get<TrainerCourseQuiz>(`${environment.baseApi}/quiz/course/${courseId}/manage`);
	}

	saveQuiz(courseId: string, model: UpsertCourseQuiz): Observable<TrainerCourseQuiz> {
		return this.http.put<TrainerCourseQuiz>(`${environment.baseApi}/quiz/course/${courseId}`, model);
	}

	getQuiz(courseId: string): Observable<CourseQuiz> {
		return this.http.get<CourseQuiz>(`${environment.baseApi}/quiz/course/${courseId}`);
	}

	submitQuiz(courseId: string, model: SubmitQuizAttempt): Observable<QuizAttemptSummary> {
		return this.http.post<QuizAttemptSummary>(`${environment.baseApi}/quiz/course/${courseId}/submit`, model);
	}

	getQuizGradebook(courseId: string): Observable<QuizAttemptSummary[]> {
		return this.http.get<QuizAttemptSummary[]>(`${environment.baseApi}/quiz/course/${courseId}/gradebook`);
	}

	getCompletionStatus(courseId: string): Observable<CourseCompletionStatus> {
		return this.http.get<CourseCompletionStatus>(
			`${environment.baseApi}/certificates/course/${courseId}/completion`
		);
	}

	issueCertificate(courseId: string): Observable<CourseCertificate> {
		return this.http.post<CourseCertificate>(
			`${environment.baseApi}/certificates/course/${courseId}/issue`,
			{}
		);
	}
}
