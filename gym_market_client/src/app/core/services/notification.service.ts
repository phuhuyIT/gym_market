import { computed, inject, Injectable, NgZone, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment.development';
import { AccountService } from '../../guest/account.service';
import { AppNotification, NotificationPreference } from '../models/notification.model';

export interface NotificationQuery {
	take?: number;
	skip?: number;
	type?: string;
	isRead?: boolean | null;
}

@Injectable({
	providedIn: 'root',
})
export class NotificationService {
	private readonly base = `${environment.baseApi}/Notifications`;
	private hubConnection: signalR.HubConnection | undefined;

	private http = inject(HttpClient);
	private ngZone = inject(NgZone);
	private accountService = inject(AccountService);

	readonly notifications = signal<AppNotification[]>([]);
	readonly unreadCount = computed(() => this.notifications().filter(n => !n.isRead).length);

	// Unread count for a single category — drives the cue on the Payments nav item.
	readonly unreadPaymentCount = computed(
		() => this.notifications().filter(n => !n.isRead && n.type === 'payment').length
	);

	constructor() {
		// Logout (token cleared) tears the hub down and empties the panel so the
		// next user on this browser doesn't see stale notifications.
		this.accountService.token$.subscribe(token => {
			if (!token) {
				this.stopConnection();
				this.notifications.set([]);
			}
		});
	}

	// Idempotent: the bell calls this every time a header mounts; an already
	// live connection is reused instead of reconnecting on each navigation.
	init() {
		if (!this.accountService.token) {
			return;
		}
		this.load();
		this.startConnection();
	}

	private startConnection() {
		if (this.hubConnection) {
			return;
		}
		const hubUrl = environment.baseApi.replace('/api', '/hubs/notifications');

		this.hubConnection = new signalR.HubConnectionBuilder()
			.withUrl(hubUrl, {
				accessTokenFactory: () => this.accountService.token ?? '',
			})
			.withAutomaticReconnect()
			.build();

		// SignalR callbacks can fire outside Angular's zone, so run them inside
		// ngZone to guarantee change detection picks up the updates.
		this.hubConnection.on('ReceiveNotification', (notification: AppNotification) => {
			this.ngZone.run(() => {
				// Upserts (e.g. chat messages) re-send an existing id — replace
				// the old entry and move it to the top instead of duplicating.
				this.notifications.update(list => [notification, ...list.filter(n => n.id !== notification.id)]);
			});
		});

		this.hubConnection.start().catch(err => console.error('Notification hub connection failed:', err));
	}

	private stopConnection() {
		if (this.hubConnection) {
			this.hubConnection.stop().catch(err => console.error('Notification hub disconnect failed:', err));
			this.hubConnection = undefined;
		}
	}

	private load() {
		this.getNotifications().subscribe({
			next: res => this.notifications.set(res),
		});
	}

	getNotifications(query: NotificationQuery = {}) {
		let params = new HttpParams();
		if (query.take !== undefined) {
			params = params.set('take', query.take);
		}
		if (query.skip !== undefined) {
			params = params.set('skip', query.skip);
		}
		if (query.type) {
			params = params.set('type', query.type);
		}
		if (query.isRead !== undefined && query.isRead !== null) {
			params = params.set('isRead', query.isRead);
		}

		return this.http.get<AppNotification[]>(`${this.base}/get-notifications`, { params });
	}

	getPreferences() {
		return this.http.get<NotificationPreference[]>(`${this.base}/preferences`);
	}

	updatePreferences(preferences: NotificationPreference[]) {
		return this.http.put<NotificationPreference[]>(`${this.base}/preferences`, {
				preferences: preferences.map(preference => ({
					type: preference.type,
					inAppEnabled: preference.inAppEnabled,
					emailEnabled: preference.emailEnabled,
				})),
			});
	}

	markRead(id: number) {
		const target = this.notifications().find(n => n.id === id);
		if (!target || target.isRead) {
			return;
		}
		// Optimistic: flip locally first so the badge reacts instantly.
		this.notifications.update(list => list.map(n => (n.id === id ? { ...n, isRead: true } : n)));
		this.http.post<void>(`${this.base}/mark-read/${id}`, {}).subscribe();
	}

	markAllRead() {
		if (this.unreadCount() === 0) {
			return;
		}
		this.notifications.update(list => list.map(n => ({ ...n, isRead: true })));
		this.http.post<void>(`${this.base}/mark-all-read`, {}).subscribe();
	}

	// Marks every unread notification of one category as read. Used when the trainer
	// opens the Payments page — seeing the list clears its nav cue.
	markTypeRead(type: string) {
		const unread = this.notifications().filter(n => !n.isRead && n.type === type);
		if (unread.length === 0) {
			return;
		}
		this.notifications.update(list => list.map(n => (n.type === type ? { ...n, isRead: true } : n)));
		this.http.post<void>(`${this.base}/mark-type-read/${encodeURIComponent(type)}`, {}).subscribe();
	}
}
