import { inject, Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Message } from '../core/models/conversation.model';
import { environment } from '../../environments/environment.development';
import { AccountService } from '../guest/account.service';

@Injectable({
	providedIn: 'root',
})
export class ChatHupService {
	private hubConnection: signalR.HubConnection | undefined;
	private accountService = inject(AccountService);

	startConnection() {
		const hubUrl = environment.baseApi.replace('/api', '/hubs/chat');

		this.hubConnection = new signalR.HubConnectionBuilder()
			.withUrl(hubUrl, {
				accessTokenFactory: () => this.accountService.token ?? '',
			})
			.withAutomaticReconnect()
			.build();

		this.hubConnection.start().catch(err => console.error('SignalR connection failed:', err));
	}

	sendMessage(roomName: string, message: Message) {
		if (this.hubConnection) {
			this.hubConnection
				.invoke('SendMessageToRoom', roomName, message)
				.catch(err => console.error('SendMessage failed:', err));
		}
	}

	onMessageReceived(callback: (message: Message) => void) {
		if (this.hubConnection) {
			this.hubConnection.on('ReceiveMessage', callback);
		}
	}

	onGroupUpdated(callback: (conversationId: number) => void) {
		if (this.hubConnection) {
			this.hubConnection.on('GroupUpdated', callback);
		}
	}

	joinGroup(roomName: string) {
		if (this.hubConnection) {
			this.hubConnection.invoke('JoinRoom', roomName).catch(err => console.error('JoinRoom failed:', err));
		}
	}

	stopConnection() {
		if (this.hubConnection) {
			this.hubConnection.stop().catch(err => console.error('SignalR disconnect failed:', err));
		}
	}
}
