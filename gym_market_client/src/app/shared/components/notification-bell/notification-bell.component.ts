import { ChangeDetectionStrategy, Component, HostListener, inject, Input, OnInit } from '@angular/core';
import { NgStyle } from '@angular/common';
import { Router } from '@angular/router';
import { NotificationService } from '../../../core/services/notification.service';
import { AppNotification } from '../../../core/models/notification.model';

@Component({
	selector: 'app-notification-bell',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [NgStyle],
	templateUrl: './notification-bell.component.html',
	styleUrl: './notification-bell.component.scss',
})
export class NotificationBellComponent implements OnInit {
	// Which side of the bell the panel hangs from. 'right' suits the page header;
	// use 'left' when the bell sits near the left edge (e.g. the chat sidebar) so
	// the panel opens inward instead of off-screen.
	@Input() align: 'right' | 'left' = 'right';

	// 'anchored' positions the panel absolutely relative to the bell. 'fixed'
	// computes viewport coordinates instead, clamped on-screen — use it when an
	// ancestor would clip the panel (e.g. the agency sidebar, a scroll container).
	@Input() mode: 'anchored' | 'fixed' = 'anchored';

	notificationService = inject(NotificationService);
	private router = inject(Router);

	showPanel = false;
	panelStyle: Record<string, string> = {};

	ngOnInit() {
		this.notificationService.init();
	}

	togglePanel(event: MouseEvent) {
		event.stopPropagation();
		this.showPanel = !this.showPanel;

		if (this.showPanel && this.mode === 'fixed') {
			const rect = (event.currentTarget as HTMLElement).getBoundingClientRect();
			// Open below the bell, left-aligned; clamp so the panel (340px wide,
			// up to 440px tall) never leaves the viewport.
			const top = Math.max(8, Math.min(rect.bottom + 8, window.innerHeight - 456));
			const left = Math.max(8, Math.min(rect.left, window.innerWidth - 356));
			this.panelStyle = {
				position: 'fixed',
				top: `${top}px`,
				left: `${left}px`,
				right: 'auto',
			};
		}
	}

	@HostListener('document:click')
	closePanel() {
		this.showPanel = false;
	}

	onNotificationClick(notification: AppNotification) {
		this.notificationService.markRead(notification.id);
		this.showPanel = false;
		if (notification.link) {
			this.router.navigateByUrl(notification.link);
		}
	}

	markAllRead(event: MouseEvent) {
		event.stopPropagation();
		this.notificationService.markAllRead();
	}

	timeAgo(dateStr: string): string {
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
}
