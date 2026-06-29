import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize, take } from 'rxjs';
import { ToastService } from '../../shared/services/toast.service';
import { AdminNotificationTemplate, NotificationDeliveryLog } from './admin-notification.model';
import { AdminNotificationsService } from './admin-notifications.service';

@Component({
	selector: 'app-admin-notifications',
	imports: [CommonModule, FormsModule],
	templateUrl: './admin-notifications.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminNotificationsComponent implements OnInit {
	private adminNotificationsService = inject(AdminNotificationsService);
	private toastService = inject(ToastService);
	private cdr = inject(ChangeDetectorRef);

	templates: AdminNotificationTemplate[] = [];
	selectedTemplate: AdminNotificationTemplate | null = null;
	editingSubject = '';
	editingBody = '';
	editingIsActive = true;
	deliveries: NotificationDeliveryLog[] = [];
	variables: string[] = [];

	typeFilter = '';
	channelFilter = '';
	statusFilter = '';
	pageIndex = 1;
	pageSize = 25;
	totalCount = 0;
	totalPages = 0;
	loadingTemplates = false;
	loadingDeliveries = false;
	saving = false;

	ngOnInit(): void {
		this.loadTemplates();
		this.loadDeliveries();
	}

	loadTemplates(): void {
		this.loadingTemplates = true;
		this.adminNotificationsService
			.getTemplates()
			.pipe(
				take(1),
				finalize(() => {
					this.loadingTemplates = false;
					this.cdr.markForCheck();
				})
			)
			.subscribe({
				next: templates => {
					this.templates = templates;
					this.variables = templates[0]?.variables ?? [];
					this.selectTemplate(this.selectedTemplate ?? templates[0] ?? null);
					this.cdr.markForCheck();
				},
				error: err => this.showError(err, 'Failed to load notification templates'),
			});
	}

	loadDeliveries(): void {
		this.loadingDeliveries = true;
		this.adminNotificationsService
			.getDeliveries({
				type: this.typeFilter,
				channel: this.channelFilter,
				status: this.statusFilter,
				pageIndex: this.pageIndex,
				pageSize: this.pageSize,
			})
			.pipe(
				take(1),
				finalize(() => {
					this.loadingDeliveries = false;
					this.cdr.markForCheck();
				})
			)
			.subscribe({
				next: result => {
					this.deliveries = result.items;
					this.totalCount = result.totalCount;
					this.totalPages = result.totalPages;
					this.cdr.markForCheck();
				},
				error: err => this.showError(err, 'Failed to load delivery history'),
			});
	}

	selectTemplate(template: AdminNotificationTemplate | null): void {
		if (!template) {
			this.selectedTemplate = null;
			return;
		}

		const fresh = this.templates.find(item => item.type === template.type) ?? template;
		this.selectedTemplate = fresh;
		this.editingSubject = fresh.subjectTemplate;
		this.editingBody = fresh.bodyTemplate;
		this.editingIsActive = fresh.isActive;
	}

	saveTemplate(): void {
		if (!this.selectedTemplate || this.saving) {
			return;
		}

		this.saving = true;
		this.adminNotificationsService
			.updateTemplate(this.selectedTemplate.type, {
				subjectTemplate: this.editingSubject,
				bodyTemplate: this.editingBody,
				isActive: this.editingIsActive,
			})
			.pipe(
				take(1),
				finalize(() => {
					this.saving = false;
					this.cdr.markForCheck();
				})
			)
			.subscribe({
				next: template => {
					this.templates = this.templates.map(item => (item.type === template.type ? template : item));
					this.selectTemplate(template);
					this.toastService.show('Notification template saved.', 'success');
					this.cdr.markForCheck();
				},
				error: err => this.showError(err, 'Failed to save template'),
			});
	}

	resetTemplate(): void {
		if (this.selectedTemplate) {
			this.selectTemplate(this.selectedTemplate);
		}
	}

	applyDeliveryFilters(): void {
		this.pageIndex = 1;
		this.loadDeliveries();
	}

	clearDeliveryFilters(): void {
		this.typeFilter = '';
		this.channelFilter = '';
		this.statusFilter = '';
		this.applyDeliveryFilters();
	}

	nextPage(): void {
		if (this.pageIndex >= this.totalPages) return;
		this.pageIndex++;
		this.loadDeliveries();
	}

	previousPage(): void {
		if (this.pageIndex <= 1) return;
		this.pageIndex--;
		this.loadDeliveries();
	}

	channelLabel(channel: string): string {
		return channel === 'in_app' ? 'Feed' : 'Email';
	}

	statusLabel(status: string): string {
		return status.charAt(0).toUpperCase() + status.slice(1);
	}

	private showError(err: unknown, fallback: string): void {
		const error = err as { error?: { errors?: string[] | string; message?: string } };
		const errors = error.error?.errors;
		const message = Array.isArray(errors) ? errors.join(', ') : errors || error.error?.message || fallback;
		this.toastService.show(message, 'error');
	}
}
