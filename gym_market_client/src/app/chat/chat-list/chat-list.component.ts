import { Component, DestroyRef, inject, OnDestroy, OnInit, ChangeDetectionStrategy, ElementRef, ViewChild, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { LoaderModalStore } from '../../stores/loader.store';
import { UserStore } from '../../stores/user.store';
import { ConversationService } from '../conversation.service';
import { patchState } from '@ngrx/signals';
import { Router } from '@angular/router';
import { AccountService } from '../../guest/account.service';

import { MessageService } from '../message.service';
import { ChatHupService } from '../chat-hup.service';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Conversation, Message } from '../../core/models/conversation.model';
import { DEFAULT_AVATAR_URL } from '../../utilities/defaults.const';

@Component({
    selector: 'app-chat-list',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [FormsModule, DatePipe],
    templateUrl: './chat-list.component.html',
    styleUrl: './chat-list.component.scss'
})
export class ChatListComponent implements OnInit, OnDestroy, AfterViewChecked {
	readonly DEFAULT_AVATAR_URL = DEFAULT_AVATAR_URL;
	chats: Conversation[] = [];
	messages: Message[] = [];
	searchQuery = '';
	activeFilter: 'all' | 'unread' | 'archive' = 'all';
	private shouldScrollToBottom = false;

	@ViewChild('messageContainer') private messageContainer!: ElementRef;

	loader = inject(LoaderModalStore);
	userStore = inject(UserStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private accountService = inject(AccountService);
	private router = inject(Router);

	conversationName: string = '';
	conversationUrl: string = '';
	roomName: string = '';
	conversationId: number = 0;

	message = '';

	get filteredChats(): Conversation[] {
		let result = this.chats;
		if (this.activeFilter === 'unread') {
			result = result.filter(c => c.hasNewMessage);
		}
		if (this.searchQuery.trim()) {
			const q = this.searchQuery.toLowerCase();
			result = result.filter(c => c.conversationName.toLowerCase().includes(q));
		}
		return result;
	}

	constructor(
		private conversationService: ConversationService,
		private messageService: MessageService,
		private chatHupService: ChatHupService
	) {}

	ngAfterViewChecked() {
		if (this.shouldScrollToBottom) {
			this.scrollToBottom();
			this.shouldScrollToBottom = false;
		}
	}

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
				this.shouldScrollToBottom = true;
			}
			this.cdr.markForCheck();
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

	clearConversation() {
		this.conversationName = '';
		this.conversationUrl = '';
		this.conversationId = 0;
		this.messages = [];
	}

	logout() {
		this.accountService.logout();
		this.router.navigateByUrl('');
	}

	getMessages(item: Conversation) {
		this.messageService
			.getMessages(item.conversationId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.messages = res;
					this.shouldScrollToBottom = true;
					this.cdr.markForCheck();
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
				this.shouldScrollToBottom = true;
			}
		}
	}

	private scrollToBottom() {
		const el = this.messageContainer?.nativeElement;
		if (el) {
			el.scrollTop = el.scrollHeight;
		}
	}

	ngOnDestroy(): void {
		this.chatHupService.stopConnection();
	}
}
