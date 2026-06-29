import { CommonModule } from '@angular/common';
import { Component, inject , ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { UserStore } from '../../../stores/user.store';
import { AccountService } from '../../../guest/account.service';
import { ThemeService } from '../../../core/services/theme.service';
import { ThemeToggleComponent } from '../theme-toggle/theme-toggle.component';
import { NotificationBellComponent } from '../notification-bell/notification-bell.component';
import { ROLES } from '../../../utilities/roles.const';
import { take } from 'rxjs';

interface NavItem {
	link: string;
	name: string;
}

@Component({
    selector: 'app-header',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLinkActive, RouterLink, CommonModule, FormsModule, ThemeToggleComponent, NotificationBellComponent],
    templateUrl: './header.component.html',
    styleUrl: './header.component.scss'
})
export class HeaderComponent {
	readonly ROLES = ROLES;
	showAccountOption = false;
	showAIDropdownMenu = false;
	courseSearchTerm = '';
	userStore = inject(UserStore);
	themeService = inject(ThemeService);

	aiMenu: NavItem[] = [
		{
			name: 'Predict body fat',
			link: '/check-bmi',
		},
		{
			name: 'Predict body fat by Image',
			link: '/predict-body-fat',
		},
	];

	constructor(private accountService: AccountService, private router: Router) {}

	get navbarItems(): NavItem[] {
		const role = this.userStore.role();
		if (role === ROLES.TRAINER) {
			return [
				{
					link: '/agency',
					name: 'Dashboard',
				},
				{
					link: '/chat/chat-list',
					name: 'Chats',
				},
			];
		} else if (role === ROLES.STUDENT) {
			return [
				{
					link: '/home',
					name: 'Home',
				},
				{
					link: '/client/find-trainer',
					name: 'Trainers',
				},
				{
					link: '/client/food-nutrition-calculator',
					name: 'Food Nutrition',
				},
				{
					link: '/client/course-registration',
					name: 'My Courses',
				},
				{
					link: '/client/membership',
					name: 'Membership',
				},
				{
					link: '/client/classes',
					name: 'Classes',
				},
				{
					link: '/client/workouts',
					name: 'Workouts',
				},
				{
					link: '/client/progress',
					name: 'Progress',
				},
			];
		} else {
			return [
				{
					link: '/home',
					name: 'Home',
				},
			];
		}
	}

	// Launches the course search page with the typed term. The course-search page
	// reads searchString/category/page from the query params, so navigating there
	// runs the search. Guests are bounced to login by the client guard (their query
	// is preserved via returnUrl).
	searchCourses() {
		this.router.navigate(['/client/course-search'], {
			queryParams: {
				searchString: this.courseSearchTerm.trim(),
				pageIndex: 1,
				pageSize: 10,
				category: 'All',
			},
		});
	}

	onShowAccountOption() {
		this.showAccountOption = true;
	}

	onHiddenAccountOption() {
		this.showAccountOption = false;
	}

	logout() {
		this.accountService.apiLogout().pipe(take(1)).subscribe();
		this.accountService.logout();
		this.router.navigateByUrl('');
	}
}
