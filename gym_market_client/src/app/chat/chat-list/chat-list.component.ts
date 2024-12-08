import { Component, inject } from '@angular/core';
import { LoaderModalStore } from '../../stores/loader.store';
import { UserStore } from '../../stores/user.store';
import { ConversationService } from '../conversation.service';
import { patchState } from '@ngrx/signals';
import { CommonModule } from '@angular/common';
import { MessageService } from '../message.service';
import { ChatHupService } from '../chat-hup.service';
import { FormsModule } from '@angular/forms';
import { Message } from '../models/message.dto';

@Component({
	selector: 'app-chat-list',
	standalone: true,
	imports: [CommonModule, FormsModule],
	templateUrl: './chat-list.component.html',
	styleUrl: './chat-list.component.scss',
})
export class ChatListComponent {
	chats: any = [];
	messages: Message[] = [];

	loader = inject(LoaderModalStore);
	userStore = inject(UserStore);

	conversationName: string = '';
	conversationUrl: string = '';
	roomName: string = '';
	conversationId: number = 0;

	message = '';

	constructor(
		private conversationService: ConversationService,
		private messageService: MessageService,
		private chatHupService: ChatHupService
	) {}

	ngOnInit() {
		this.getConversations();

		this.chatHupService.startConnection();
		this.chatHupService.onMessageReceived((message: Message) => {
			const chatConver = this.chats.find(
				(c: Message) => c.conversationId === message.conversationId
			);
			if (chatConver) {
				chatConver.lastMessage = message.content;
				if (message.senderId !== this.userStore.id()) {
					chatConver.hasNewMessage = true;
				}
			}

			this.messages.push({
				avatar: message.avatar,
				content: message.content,
				conversationId: message.conversationId,
				senderId: message.senderId,
			});
		});
	}

	private getConversations() {
		patchState(this.loader, { isShow: true });
		if (this.userStore.id() !== null) {
			this.conversationService.getConversations(this.userStore.id()).subscribe({
				next: (res: any) => {
					// console.log(res);
					patchState(this.loader, { isShow: false });
					this.chats = res;
				},
				error: err => {
					console.log(err);
					patchState(this.loader, { isShow: false });
				},
			});
		}
	}

	getMessages(item: any) {
		this.messageService.getMessages(item.conversationId).subscribe({
			next: (res: any) => {
				this.messages = res;
				// console.log(res);
			},
			error: err => {
				console.log(err);
			},
		});

		if (this.userStore.id() !== null) {
			this.messageService.seenMessage(this.userStore.id(), item.conversationId).subscribe({
				next: _ => {},
				error: err => {
					console.log(err);
				},
			});
		}

		item.hasNewMessage = false;

		this.chatHupService.joinGroup(item.conversationId.toString());
		this.conversationName = item.conversitionName;
		this.conversationUrl = item.avatar;
		this.conversationId = item.conversationId;
		this.roomName = item.conversationId.toString();
	}

	sendMessage(): void {
		if (this.message) {
			this.chatHupService.sendMessage(this.roomName, {
				avatar: this.userStore.avatar(),
				content: this.message,
				conversationId: this.conversationId,
				senderId: this.userStore.id(),
			});
			this.message = ''; // Clear message input
		}
	}

	// Hủy kết nối khi component bị phá hủy
	ngOnDestroy(): void {
		this.chatHupService.stopConnection();
	}
}
