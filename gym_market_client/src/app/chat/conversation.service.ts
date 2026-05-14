import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Conversation, CreateConversationDto } from '../core/models/conversation.model';
import { ApiResponse } from '../core/models/auth.model';

@Injectable({
	providedIn: 'root',
})
export class ConversationService {
	constructor(private http: HttpClient) {}

	createConversation(model: CreateConversationDto): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${environment.baseApi}/Conversations/create-conversation`, model);
	}

	getConversations(userId: string | null): Observable<Conversation[]> {
		return this.http.get<Conversation[]>(
			`${environment.baseApi}/Conversations/GetConversationOfUser/${userId}`
		);
	}
}
