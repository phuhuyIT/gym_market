import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize, take } from 'rxjs';
import { ToastService } from '../../shared/services/toast.service';
import { AdminUser, AdminUserDetail } from './admin-user.model';
import { AdminUsersService } from './admin-users.service';

@Component({
	selector: 'app-admin-users',
	imports: [CommonModule, FormsModule],
	templateUrl: './admin-users.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminUsersComponent implements OnInit {
	private adminUsersService = inject(AdminUsersService);
	private toastService = inject(ToastService);
	private cdr = inject(ChangeDetectorRef);

	users: AdminUser[] = [];
	selectedUser: AdminUserDetail | null = null;
	search = '';
	role = '';
	status = '';
	trainerApprovalStatus = '';
	emailConfirmed = '';
	pageIndex = 1;
	pageSize = 10;
	totalCount = 0;
	totalPages = 0;
	loading = false;
	actionUserId = '';

	ngOnInit(): void {
		this.loadUsers();
	}

	loadUsers(): void {
		this.loading = true;
		this.adminUsersService
			.searchUsers({
				search: this.search,
					role: this.role,
					status: this.status,
					trainerApprovalStatus: this.trainerApprovalStatus,
					emailConfirmed: this.emailConfirmed,
				pageIndex: this.pageIndex,
				pageSize: this.pageSize,
			})
			.pipe(
				take(1),
				finalize(() => {
					this.loading = false;
					this.cdr.markForCheck();
				})
			)
			.subscribe({
				next: result => {
					this.users = result.items;
					this.totalCount = result.totalCount;
					this.totalPages = result.totalPages;
					if (this.selectedUser && !this.users.some(user => user.id === this.selectedUser?.id)) {
						this.selectedUser = null;
					}
					this.cdr.markForCheck();
				},
				error: err => this.showError(err, 'Failed to load users'),
			});
	}

	applyFilters(): void {
		this.pageIndex = 1;
		this.loadUsers();
	}

	clearFilters(): void {
		this.search = '';
		this.role = '';
		this.status = '';
		this.trainerApprovalStatus = '';
		this.emailConfirmed = '';
		this.applyFilters();
	}

	nextPage(): void {
		if (this.pageIndex >= this.totalPages) return;
		this.pageIndex++;
		this.loadUsers();
	}

	previousPage(): void {
		if (this.pageIndex <= 1) return;
		this.pageIndex--;
		this.loadUsers();
	}

	selectUser(user: AdminUser): void {
		this.actionUserId = user.id;
		this.adminUsersService
			.getUser(user.id)
			.pipe(
				take(1),
				finalize(() => {
					this.actionUserId = '';
					this.cdr.markForCheck();
				})
			)
			.subscribe({
				next: detail => {
					this.selectedUser = detail;
					this.cdr.markForCheck();
				},
				error: err => this.showError(err, 'Failed to load user detail'),
			});
	}

	setStatus(user: AdminUser, status: 'Active' | 'Suspended'): void {
		this.actionUserId = user.id;
		this.adminUsersService
			.updateStatus(user.id, status)
			.pipe(
				take(1),
				finalize(() => {
					this.actionUserId = '';
					this.cdr.markForCheck();
				})
			)
			.subscribe({
				next: () => {
					this.toastService.show(`User marked ${status.toLowerCase()}.`, 'success');
					this.loadUsers();
					if (this.selectedUser?.id === user.id) this.selectUser(user);
				},
				error: err => this.showError(err, 'Failed to update status'),
			});
	}

	setTrainerApproval(user: AdminUser, status: 'PendingReview' | 'Approved' | 'Rejected'): void {
		if (!user.trainerId) return;

		this.actionUserId = user.id;
		this.adminUsersService
			.updateTrainerApproval(user.id, status)
			.pipe(
				take(1),
				finalize(() => {
					this.actionUserId = '';
					this.cdr.markForCheck();
				})
			)
			.subscribe({
				next: () => {
					this.toastService.show(`Trainer marked ${this.trainerApprovalLabel(status).toLowerCase()}.`, 'success');
					this.loadUsers();
					if (this.selectedUser?.id === user.id) this.selectUser(user);
				},
				error: err => this.showError(err, 'Failed to update trainer approval'),
			});
	}

	trainerApprovalLabel(status?: string | null): string {
		switch (status) {
			case 'Approved':
				return 'Approved';
			case 'Rejected':
				return 'Rejected';
			case 'PendingReview':
				return 'Pending review';
			default:
				return 'Not applicable';
		}
	}

	resendConfirmation(user: AdminUser): void {
		this.actionUserId = user.id;
		this.adminUsersService
			.resendConfirmation(user.id)
			.pipe(
				take(1),
				finalize(() => {
					this.actionUserId = '';
					this.cdr.markForCheck();
				})
			)
			.subscribe({
				next: () => this.toastService.show('Confirmation email sent.', 'success'),
				error: err => this.showError(err, 'Failed to send confirmation email'),
			});
	}

	unlock(user: AdminUser): void {
		this.actionUserId = user.id;
		this.adminUsersService
			.unlock(user.id)
			.pipe(
				take(1),
				finalize(() => {
					this.actionUserId = '';
					this.cdr.markForCheck();
				})
			)
			.subscribe({
				next: () => {
					this.toastService.show('Account unlocked.', 'success');
					this.loadUsers();
					if (this.selectedUser?.id === user.id) this.selectUser(user);
				},
				error: err => this.showError(err, 'Failed to unlock account'),
			});
	}

	private showError(err: unknown, fallback: string): void {
		const error = err as { error?: { errors?: string[] | string; message?: string } };
		const errors = error.error?.errors;
		const message = Array.isArray(errors) ? errors.join(', ') : errors || error.error?.message || fallback;
		this.toastService.show(message, 'error');
	}
}
