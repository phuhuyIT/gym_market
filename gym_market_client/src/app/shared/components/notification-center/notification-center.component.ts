import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AppNotification } from '../../../core/models/notification.model';
import { NotificationQuery, NotificationService } from '../../../core/services/notification.service';

type ReadFilter = 'all' | 'unread' | 'read';

interface NotificationCategory {
	type: string;
	label: string;
}

@Component({
	selector: 'app-notification-center',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [DatePipe],
	templateUrl: './notification-center.component.html',
	styleUrl: './notification-center.component.scss',
})
export class NotificationCenterComponent implements OnInit {
	private readonly pageSize = 25;
	private readonly router = inject(Router);
	readonly notificationService = inject(NotificationService);

	readonly categories: NotificationCategory[] = [
		{ type: 'all', label: 'All' },
		{ type: 'class', label: 'Class' },
		{ type: 'workout', label: 'Workout' },
		{ type: 'progress', label: 'Progress' },
		{ type: 'membership', label: 'Membership' },
		{ type: 'payment', label: 'Payment' },
		{ type: 'course', label: 'Course' },
		{ type: 'chat', label: 'Chat' },
		{ type: 'system', label: 'System' },
	];
	readonly readFilters: Array<{ value: ReadFilter; label: string }> = [
		{ value: 'all', label: 'All' },
		{ value: 'unread', label: 'Unread' },
		{ value: 'read', label: 'Read' },
	];

	readonly selectedType = signal('all');
	readonly readFilter = signal<ReadFilter>('all');
	readonly notifications = signal<AppNotification[]>([]);
	readonly loading = signal(false);
	readonly hasMore = signal(false);

	readonly selectedLabel = computed(() => this.categoryLabel(this.selectedType()));
	readonly unreadInView = computed(() => this.notifications().filter(n => !n.isRead).length);
	readonly canMarkScopeRead = computed(() => this.notifications().some(n => !n.isRead));

	ngOnInit() {
		this.notificationService.init();
		this.load(true);
	}

	selectCategory(type: string) {
		if (this.selectedType() === type) {
			return;
		}
		this.selectedType.set(type);
		this.load(true);
	}

	selectReadFilter(value: ReadFilter) {
		if (this.readFilter() === value) {
			return;
		}
		this.readFilter.set(value);
		this.load(true);
	}

	load(reset = false) {
		if (this.loading()) {
			return;
		}

		const query = this.query(reset);
		this.loading.set(true);
		this.notificationService
			.getNotifications(query)
			.pipe(finalize(() => this.loading.set(false)))
			.subscribe({
				next: res => {
					const page = res.slice(0, this.pageSize);
					this.hasMore.set(res.length > this.pageSize);
					this.notifications.set(reset ? page : [...this.notifications(), ...page]);
				},
			});
	}

	openNotification(notification: AppNotification) {
		if (!notification.isRead) {
			this.notificationService.markRead(notification.id);
			this.notifications.update(list =>
				list.map(item => (item.id === notification.id ? { ...item, isRead: true } : item))
			);
		}

		if (notification.link) {
			this.router.navigateByUrl(notification.link);
		}
	}

	markScopeRead() {
		if (!this.canMarkScopeRead()) {
			return;
		}

		const selected = this.selectedType();
		if (selected === 'all') {
			this.notificationService.markAllRead();
		} else {
			this.notificationService.markTypeRead(selected);
		}

		this.notifications.update(list =>
			list.map(item => (selected === 'all' || item.type === selected ? { ...item, isRead: true } : item))
		);

		if (this.readFilter() === 'unread') {
			this.load(true);
		}
	}

	categoryUnreadCount(type: string): number {
		return this.notificationService
			.notifications()
			.filter(n => !n.isRead && (type === 'all' || n.type === type)).length;
	}

	categoryLabel(type: string): string {
		return this.categories.find(category => category.type === type)?.label ?? this.toTitleCase(type);
	}

	trackByCategory(_: number, category: NotificationCategory) {
		return category.type;
	}

	private query(reset: boolean): NotificationQuery {
		const selected = this.selectedType();
		const filter = this.readFilter();
		return {
			take: this.pageSize + 1,
			skip: reset ? 0 : this.notifications().length,
			type: selected === 'all' ? undefined : selected,
			isRead: filter === 'all' ? null : filter === 'read',
		};
	}

	private toTitleCase(value: string): string {
		return value ? value.charAt(0).toUpperCase() + value.slice(1) : 'Notification';
	}
}
