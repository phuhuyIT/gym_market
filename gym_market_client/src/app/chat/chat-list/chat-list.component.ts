import { Component, DestroyRef, inject, OnDestroy, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { LoaderModalStore } from '../../stores/loader.store';
import { UserStore } from '../../stores/user.store';
import { ConversationService } from '../conversation.service';
import { patchState } from '@ngrx/signals';

import { MessageService } from '../message.service';
import { ChatHupService } from '../chat-hup.service';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Conversation, Message } from '../../core/models/conversation.model';
import { GmInputComponent } from '../../shared';
import { DEFAULT_AVATAR_URL } from '../../utilities/defaults.const';

@Component({
    selector: 'app-chat-list',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [FormsModule, GmInputComponent],
    templateUrl: './chat-list.component.html',
    styleUrl: './chat-list.component.scss'
})
export class ChatListComponent implements OnInit, OnDestroy {
	readonly DEFAULT_AVATAR_URL = DEFAULT_AVATAR_URL;
	chats: Conversation[] = [];
	messages: Message[] = [];

	loader = inject(LoaderModalStore);
	userStore = inject(UserStore);
	private destroyRef = inject(DestroyRef);

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
				(c: Conversation) => c.conversationId === message.conversationId
			);
			if (chatConver) {
				chatConver.lastMessage = message.content;
				if (message.senderId !== this.userStore.id()) {
					chatConver.hasNewMessage = true;
				}
			}

			if (message.conversationId === this.conversationId) {
				this.messages.push({
					avatar: message.avatar,
					content: message.content,
					conversationId: message.conversationId,
					senderId: message.senderId,
				});
			}
		});
	}

	private getConversations() {
		patchState(this.loader, { isShow: true });
		const userId = this.userStore.id();
		if (userId !== null) {
			this.conversationService
				.getConversations(userId)
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: res => {
						patchState(this.loader, { isShow: false });
						this.chats = res;
					},
					error: () => {
						patchState(this.loader, { isShow: false });
					},
				});
		}
	}

	getMessages(item: Conversation) {
		this.messageService
			.getMessages(item.conversationId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.messages = res;
				},
			});

		const userId = this.userStore.id();
		if (userId !== null) {
			this.messageService
				.seenMessage(userId, item.conversationId)
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: () => {},
				});
		}

		item.hasNewMessage = false;

		this.chatHupService.joinGroup(item.conversationId.toString());
		this.conversationName = item.conversationName;
		this.conversationUrl = item.avatar;
		this.conversationId = item.conversationId;
		this.roomName = item.conversationId.toString();
	}

	sendMessage(): void {
		if (this.message) {
			const userId = this.userStore.id();
			if (userId) {
				this.chatHupService.sendMessage(this.roomName, {
					avatar: this.userStore.avatar() ?? '',
					content: this.message,
					conversationId: this.conversationId,
					senderId: userId,
				});
				this.message = '';
			}
		}
	}

	ngOnDestroy(): void {
		this.chatHupService.stopConnection();
	}
}
