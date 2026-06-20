import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserStore } from '../stores/user.store';
import { AccountService } from '../guest/account.service';
import { ThemeToggleComponent } from '../shared/components/theme-toggle/theme-toggle.component';
import { NotificationBellComponent } from '../shared/components/notification-bell/notification-bell.component';

import { NotificationService } from '../core/services/notification.service';
import { CourseAgencyService } from './course-agency.service';
import { FormsModule } from '@angular/forms';
import { take } from 'rxjs';
import { ROLES } from '../utilities/roles.const';

@Component({
    selector: 'app-course-agency',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule, ThemeToggleComponent, FormsModule, NotificationBellComponent],
    templateUrl: './course-agency.component.html',
    styleUrl: './course-agency.component.scss'
})
export class CourseAgencyComponent {
	userStore = inject(UserStore);
	courseAgencyService = inject(CourseAgencyService);
	notificationService = inject(NotificationService);
	private accountService = inject(AccountService);
	private router = inject(Router);

	isMobileMenuOpen = signal<boolean>(false);
	showProfileDropdown = signal<boolean>(false);

	get searchVal(): string {
		return this.courseAgencyService.searchString();
	}
	set searchVal(val: string) {
		this.courseAgencyService.searchString.set(val);
	}

	get showSearchBar(): boolean {
		return false;
	}

	get isAdmin(): boolean {
		return this.accountService.getRole() === ROLES.ADMIN;
	}

	get pageTitle(): string {
		const url = this.router.url;
		if (url.includes('/dashboard') || url === '/agency') return 'Overview';
		if (url.includes('/add-course')) return 'Add New Course';
		if (url.includes('/update-course')) return 'Edit Course';
		if (url.includes('/course-materials')) return 'Course Materials';
		if (url.includes('/course-option-list')) return 'Course Options';
		if (url.includes('/courses')) return 'Manage Courses';
		if (url.includes('/students')) return 'Manage Students';
		if (url.includes('/payments')) return 'Payments';
		if (url.includes('/nutrition')) return 'Food Database';
		if (url.includes('/your-profile')) return 'Profile Details';
		if (url.includes('/edit-profile')) return 'Edit Profile';
		if (url.includes('/account-settings')) return 'Account Settings';
		if (url.includes('/chat')) return 'Community Chat';
		return 'Trainer Dashboard';
	}

	toggleMobileMenu() {
		this.isMobileMenuOpen.update(v => !v);
	}

	closeMobileMenu() {
		this.isMobileMenuOpen.set(false);
	}

	toggleProfileDropdown() {
		this.showProfileDropdown.update(v => !v);
	}

	logout() {
		this.accountService.apiLogout().pipe(take(1)).subscribe();
		this.accountService.logout();
		this.router.navigateByUrl('');
	}
}
