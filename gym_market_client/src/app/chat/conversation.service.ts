import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';

@Injectable({
	providedIn: 'root',
})
export class ConversationService {
	constructor(private http: HttpClient) {}

	createConversation(model: any) {
		return this.http.post(`${environment.baseApi}/Conversations/create-conversation`, model);
	}

	getConversations(userId: string | null) {
		return this.http.get(`${environment.baseApi}/Conversations/GetConversationOfUser/${userId}`);
	}
}
