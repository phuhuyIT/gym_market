import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';

@Injectable({
	providedIn: 'root',
})
export class MessageService {
	constructor(private http: HttpClient) {}

	getMessages(conversationId: number) {
		return this.http.get(`${environment.baseApi}/Conversations/get-messages/${conversationId}`);
	}

    seenMessage(userId: string | null, conversationId: number) { 
        return this.http.get(`${environment.baseApi}/Conversations/seen-message/${userId}/${conversationId}`);
    }
}
