import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ThemeToggleComponent } from '../shared/components/theme-toggle/theme-toggle.component';
import { NotificationBellComponent } from '../shared/components/notification-bell/notification-bell.component';
import { UserStore } from '../stores/user.store';
import { AccountService } from '../guest/account.service';
import { take } from 'rxjs';

@Component({
	selector: 'app-admin',
	imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, ThemeToggleComponent, NotificationBellComponent],
	templateUrl: './admin.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminComponent {
	userStore = inject(UserStore);
	private accountService = inject(AccountService);
	private router = inject(Router);
	isMobileMenuOpen = signal(false);

	navItems = [
		{ label: 'Overview', path: '/admin/dashboard' },
		{ label: 'Courses', path: '/admin/courses' },
		{ label: 'Students', path: '/admin/students' },
		{ label: 'Payments', path: '/admin/payments' },
		{ label: 'Food Database', path: '/admin/nutrition' },
		{ label: 'Course Options', path: '/admin/course-option-list' },
		{ label: 'Account Settings', path: '/admin/account-settings' },
	];

	logout(): void {
		this.accountService.apiLogout().pipe(take(1)).subscribe();
		this.accountService.logout();
		this.router.navigateByUrl('');
	}
}
