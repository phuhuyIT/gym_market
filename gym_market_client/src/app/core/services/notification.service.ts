import { computed, inject, Injectable, NgZone, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment.development';
import { AccountService } from '../../guest/account.service';
import { AppNotification } from '../models/notification.model';

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
		this.http.get<AppNotification[]>(`${this.base}/get-notifications`).subscribe({
			next: res => this.notifications.set(res),
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
}
