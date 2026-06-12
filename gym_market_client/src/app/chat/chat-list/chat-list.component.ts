import { Component, DestroyRef, inject, OnDestroy, OnInit, ChangeDetectionStrategy, ElementRef, ViewChild, AfterViewChecked, ChangeDetectorRef, NgZone } from '@angular/core';
import { LoaderModalStore } from '../../stores/loader.store';
import { UserStore } from '../../stores/user.store';
import { ConversationService } from '../conversation.service';
import { patchState } from '@ngrx/signals';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { AccountService } from '../../guest/account.service';

import { MessageService } from '../message.service';
import { ChatHupService } from '../chat-hup.service';
import { FormsModule } from '@angular/forms';
import { DatePipe, CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Conversation, Message } from '../../core/models/conversation.model';
import { DEFAULT_AVATAR_URL } from '../../utilities/defaults.const';
import { CreateGroupComponent } from '../create-group/create-group.component';
import { GroupMembersComponent } from '../group-members/group-members.component';
import { NewConversationComponent } from '../new-conversation/new-conversation.component';
import { NotificationBellComponent } from '../../shared/components/notification-bell/notification-bell.component';

@Component({
    selector: 'app-chat-list',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [FormsModule, DatePipe, CommonModule, CreateGroupComponent, GroupMembersComponent, NewConversationComponent, NotificationBellComponent],
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
	private ngZone = inject(NgZone);
	private accountService = inject(AccountService);
	private router = inject(Router);
	private route = inject(ActivatedRoute);
	isAgency = false;

	conversationName: string = '';
	conversationUrl: string = '';
	roomName: string = '';
	conversationId: number = 0;

	activeIsGroup = false;
	activeRole = '';
	activeMemberCount = 0;
	activeOtherUserId: string | null = null;
	activeIsOnline = false;
	activeLastSeen: string | null = null;

	showCreateGroup = false;
	showNewConversation = false;
	showMembers = false;

	message = '';

	// Latest query params, applied once conversations are loaded. Kept outside the
	// snapshot so notification clicks that only change the query string (the
	// component is reused, not remounted) still switch the conversation.
	private routeParams: Params = {};

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
		this.isAgency = this.router.url.includes('/agency');
		this.getConversations();

		this.route.queryParams
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe(params => {
				this.routeParams = params;
				this.applyRouteTarget();
			});

		this.chatHupService.startConnection();
		// SignalR callbacks can fire outside Angular's zone, so run them inside
		// ngZone to guarantee change detection picks up the updates.
		this.chatHupService.onMessageReceived((message: Message) => {
			this.ngZone.run(() => {
				const chatConver = this.chats.find(
					(c: Conversation) => c.conversationId === message.conversationId
				);
				if (chatConver) {
					chatConver.lastMessage = message.content;
					chatConver.lastMessageAt = message.sentAt ?? new Date().toISOString();
					if (message.senderId !== this.userStore.id() && message.conversationId !== this.conversationId) {
						chatConver.hasNewMessage = true;
					}
				}

				if (message.conversationId === this.conversationId) {
					this.messages.push({
						id: message.id,
						avatar: message.avatar,
						content: message.content,
						conversationId: message.conversationId,
						senderId: message.senderId,
						senderName: message.senderName,
						sentAt: message.sentAt,
						type: message.type,
					});
					this.shouldScrollToBottom = true;
				}
				this.cdr.markForCheck();
			});
		});

		this.chatHupService.onGroupUpdated(() => {
			this.ngZone.run(() => this.getConversations());
		});

		this.chatHupService.onUserOnline((userId: string) => {
			this.ngZone.run(() => this.applyPresence(userId, true, null));
		});

		this.chatHupService.onUserOffline((userId: string, lastSeen: string) => {
			this.ngZone.run(() => this.applyPresence(userId, false, lastSeen));
		});
	}

	private applyPresence(userId: string, isOnline: boolean, lastSeen: string | null) {
		for (const chat of this.chats) {
			if (chat.otherUserId === userId) {
				chat.isOnline = isOnline;
				if (lastSeen) {
					chat.lastSeen = lastSeen;
				}
			}
		}

		if (this.activeOtherUserId === userId) {
			this.activeIsOnline = isOnline;
			if (lastSeen) {
				this.activeLastSeen = lastSeen;
			}
		}
		this.cdr.markForCheck();
	}

	// Compact relative time for the conversation list, rendered in the
	// viewer's local timezone (timestamps arrive as UTC ISO strings).
	timeAgo(dateStr: string | null | undefined): string {
		if (!dateStr) {
			return '';
		}
		const then = new Date(dateStr).getTime();
		if (isNaN(then)) {
			return '';
		}
		const minutes = Math.floor((Date.now() - then) / 60000);
		if (minutes < 1) {
			return 'Just now';
		}
		if (minutes < 60) {
			return `${minutes}m ago`;
		}
		const hours = Math.floor(minutes / 60);
		if (hours < 24) {
			return `${hours}h ago`;
		}
		const days = Math.floor(hours / 24);
		if (days < 7) {
			return `${days}d ago`;
		}
		return new Date(dateStr).toLocaleDateString();
	}

	lastSeenText(lastSeen: string | null): string {
		if (!lastSeen) {
			return 'Offline';
		}
		const then = new Date(lastSeen).getTime();
		if (isNaN(then)) {
			return 'Offline';
		}
		const diffMs = Date.now() - then;
		const minutes = Math.floor(diffMs / 60000);
		if (minutes < 1) {
			return 'Last seen just now';
		}
		if (minutes < 60) {
			return `Last seen ${minutes} minute${minutes === 1 ? '' : 's'} ago`;
		}
		const hours = Math.floor(minutes / 60);
		if (hours < 24) {
			return `Last seen ${hours} hour${hours === 1 ? '' : 's'} ago`;
		}
		const days = Math.floor(hours / 24);
		if (days < 7) {
			return `Last seen ${days} day${days === 1 ? '' : 's'} ago`;
		}
		return `Last seen on ${new Date(lastSeen).toLocaleDateString()}`;
	}

	private getConversations() {
		patchState(this.loader, { isShow: true });
		// Stay logged-in gated, but the backend identifies the user from the JWT.
		const userId = this.userStore.id();
		if (userId !== null) {
			this.conversationService
				.getConversations()
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: res => {
						patchState(this.loader, { isShow: false });
						this.chats = res;
						this.syncActiveConversation();
						this.cdr.markForCheck();
						this.applyRouteTarget();
					},
					error: () => {
						patchState(this.loader, { isShow: false });
						this.cdr.markForCheck();
					},
				});
		}
	}

	// Keeps the open chat's header in step with the refreshed list, so a group
	// rename, photo change or membership change (GroupUpdated) shows up without
	// reopening the conversation.
	private syncActiveConversation() {
		if (!this.conversationId) {
			return;
		}
		const chat = this.chats.find(c => c.conversationId === this.conversationId);
		if (chat) {
			this.conversationName = chat.conversationName;
			this.conversationUrl = chat.avatar;
			this.activeRole = chat.role ?? '';
			this.activeMemberCount = chat.memberCount ?? 0;
		}
	}

	// Opens the conversation the query params point at (conversationId from a bell
	// notification, userId/studentId from profile/manage-students deep links).
	// No-op until conversations have loaded; params are consumed so later list
	// refreshes (e.g. GroupUpdated) don't re-trigger the selection.
	private applyRouteTarget() {
		if (this.chats.length === 0) {
			return;
		}
		const conversationId = Number(this.routeParams['conversationId']);
		const targetUserId = this.routeParams['userId'] || this.routeParams['studentId'];
		this.routeParams = {};

		if (conversationId) {
			const chat = this.chats.find(c => c.conversationId === conversationId);
			if (chat) {
				this.getMessages(chat);
			}
		} else if (targetUserId) {
			this.selectConversationByUserId(targetUserId);
		}
	}

	private selectConversationByUserId(targetUserId: string) {
		const existingChat = this.chats.find(c => c.otherUserId === targetUserId);
		if (existingChat) {
			this.getMessages(existingChat);
		} else {
			const senderId = this.userStore.id();
			if (senderId) {
				patchState(this.loader, { isShow: true });
				this.conversationService.createConversation({
					recieveId: targetUserId
				}).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
					next: () => {
						this.conversationService.getConversations()
							.pipe(takeUntilDestroyed(this.destroyRef))
							.subscribe({
								next: res => {
									patchState(this.loader, { isShow: false });
									this.chats = res;
									const newChat = this.chats.find(c => c.otherUserId === targetUserId);
									if (newChat) {
										this.getMessages(newChat);
									}
									this.cdr.markForCheck();
								},
								error: () => {
									patchState(this.loader, { isShow: false });
								}
							});
					},
					error: () => {
						patchState(this.loader, { isShow: false });
					}
				});
			}
		}
	}

	clearConversation() {
		this.conversationName = '';
		this.conversationUrl = '';
		this.conversationId = 0;
		this.messages = [];
		this.activeIsGroup = false;
		this.activeRole = '';
		this.activeMemberCount = 0;
		this.activeOtherUserId = null;
		this.activeIsOnline = false;
		this.activeLastSeen = null;
		this.showMembers = false;
	}

	openCreateGroup() {
		this.showCreateGroup = true;
	}

	onGroupCreated() {
		this.showCreateGroup = false;
		this.getConversations();
	}

	openNewConversation() {
		this.showNewConversation = true;
	}

	onConversationCreated() {
		this.showNewConversation = false;
		this.getConversations();
	}

	openMembers() {
		if (this.activeIsGroup) {
			this.showMembers = true;
		}
	}

	onLeftGroup() {
		this.showMembers = false;
		this.clearConversation();
		this.getConversations();
	}

	onMembersChanged() {
		this.getConversations();
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
				.seenMessage(item.conversationId)
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
		this.activeIsGroup = item.isGroup;
		this.activeRole = item.role ?? '';
		this.activeMemberCount = item.memberCount ?? 0;
		this.activeOtherUserId = item.otherUserId ?? null;
		this.activeIsOnline = item.isOnline ?? false;
		this.activeLastSeen = item.lastSeen ?? null;
		this.showMembers = false;
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
