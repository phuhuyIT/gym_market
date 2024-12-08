import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Message } from './models/message.dto';

@Injectable({
	providedIn: 'root',
})
export class ChatHupService {
	private hubConnection: signalR.HubConnection | undefined;

	constructor() {}

	// Kết nối đến SignalR Hub
	startConnection() {
		this.hubConnection = new signalR.HubConnectionBuilder()
			.withUrl('https://localhost:7115/hubs/chat')
			.build();

		this.hubConnection
			.start()
			.then(() => console.log('SignalR connection started...'))
			.catch(err => console.log('Error while starting connection: ' + err));
	}

	// Gửi tin nhắn đến Server
	sendMessage(roomName: string, message: Message) {
		if (this.hubConnection) {
			this.hubConnection
				.invoke('SendMessageToRoom', roomName, message)
				.catch(err => console.error(err));
		}
	}

	// Nhận tin nhắn từ Server
	onMessageReceived(callback: (message: Message) => void) {
		if (this.hubConnection) {
			this.hubConnection.on('ReceiveMessage', callback);
		}
	}

	joinGroup(roomName: string) {
		if (this.hubConnection) {
			this.hubConnection
				.invoke('JoinRoom', roomName)
				.then(() => {
					console.log('join thanh công');
				})
				.catch(err => console.error(err));
		}
	}

	// Hủy kết nối SignalR
	stopConnection() {
		if (this.hubConnection) {
			this.hubConnection
				.stop()
				.then(() => console.log('SignalR connection stopped.'))
				.catch(err => console.error('Error while stopping connection: ' + err));
		}
	}
}
