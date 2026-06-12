import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { Message } from '../core/models/conversation.model';

@Injectable({
	providedIn: 'root',
})
export class MessageService {
	constructor(private http: HttpClient) {}

	getMessages(conversationId: number): Observable<Message[]> {
		return this.http.get<Message[]>(
			`${environment.baseApi}/Conversations/get-messages/${conversationId}`
		);
	}

	// The backend resolves the user from the JWT.
	seenMessage(conversationId: number): Observable<void> {
		return this.http.post<void>(
			`${environment.baseApi}/Conversations/seen-message/${conversationId}`,
			{}
		);
	}
}
