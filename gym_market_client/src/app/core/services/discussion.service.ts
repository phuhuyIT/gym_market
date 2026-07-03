import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import {
	CreateDiscussionAnswer,
	CreateDiscussionQuestion,
	DiscussionAnswer,
	DiscussionQuestion,
	ModerateDiscussionQuestion,
} from '../models/discussion.model';

@Injectable({
	providedIn: 'root',
})
export class DiscussionService {
	constructor(private http: HttpClient) {}

	getCourseQuestions(courseId: string, filters?: { status?: string; search?: string }): Observable<DiscussionQuestion[]> {
		let params = new HttpParams();
		if (filters?.status) params = params.set('status', filters.status);
		if (filters?.search?.trim()) params = params.set('search', filters.search.trim());

		return this.http.get<DiscussionQuestion[]>(`${environment.baseApi}/CourseDiscussions/course/${courseId}`, { params });
	}

	getQuestion(questionId: string): Observable<DiscussionQuestion> {
		return this.http.get<DiscussionQuestion>(`${environment.baseApi}/CourseDiscussions/questions/${questionId}`);
	}

	createQuestion(courseId: string, model: CreateDiscussionQuestion): Observable<DiscussionQuestion> {
		return this.http.post<DiscussionQuestion>(`${environment.baseApi}/CourseDiscussions/course/${courseId}/questions`, model);
	}

	createAnswer(questionId: string, model: CreateDiscussionAnswer): Observable<DiscussionAnswer> {
		return this.http.post<DiscussionAnswer>(`${environment.baseApi}/CourseDiscussions/questions/${questionId}/answers`, model);
	}

	acceptAnswer(questionId: string, answerId: string): Observable<DiscussionQuestion> {
		return this.http.put<DiscussionQuestion>(`${environment.baseApi}/CourseDiscussions/questions/${questionId}/accept/${answerId}`, {});
	}

	moderateQuestion(questionId: string, model: ModerateDiscussionQuestion): Observable<DiscussionQuestion> {
		return this.http.put<DiscussionQuestion>(`${environment.baseApi}/CourseDiscussions/questions/${questionId}/moderation`, model);
	}

	deleteQuestion(questionId: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/CourseDiscussions/questions/${questionId}`);
	}

	deleteAnswer(answerId: string): Observable<void> {
		return this.http.delete<void>(`${environment.baseApi}/CourseDiscussions/answers/${answerId}`);
	}
}
