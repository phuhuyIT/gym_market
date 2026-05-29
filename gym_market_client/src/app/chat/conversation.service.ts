import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import {
	AddMembersDto,
	Conversation,
	CreateConversationDto,
	CreateGroupDto,
	GroupMember,
	UpdateMemberRoleDto,
	UserSearchResult,
} from '../core/models/conversation.model';
import { ApiResponse } from '../core/models/auth.model';

@Injectable({
	providedIn: 'root',
})
export class ConversationService {
	private readonly base = `${environment.baseApi}/Conversations`;

	constructor(private http: HttpClient) {}

	createConversation(model: CreateConversationDto): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${this.base}/create-conversation`, model);
	}

	getConversations(userId: string | null): Observable<Conversation[]> {
		return this.http.get<Conversation[]>(`${this.base}/get-conversation-of-user/${userId}`);
	}

	createGroup(model: CreateGroupDto): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${this.base}/create-group`, model);
	}

	addMembers(model: AddMembersDto): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${this.base}/add-members`, model);
	}

	removeMember(conversationId: number, userId: string): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${this.base}/remove-member/${conversationId}/${userId}`, {});
	}

	leaveGroup(conversationId: number): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${this.base}/leave-group/${conversationId}`, {});
	}

	updateMemberRole(model: UpdateMemberRoleDto): Observable<ApiResponse> {
		return this.http.post<ApiResponse>(`${this.base}/update-member-role`, model);
	}

	getGroupMembers(conversationId: number): Observable<GroupMember[]> {
		return this.http.get<GroupMember[]>(`${this.base}/group-members/${conversationId}`);
	}

	searchUsers(query: string): Observable<UserSearchResult[]> {
		return this.http.get<UserSearchResult[]>(`${this.base}/search-users`, {
			params: { query: query ?? '' },
		});
	}
}
