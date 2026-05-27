import { CommonModule } from '@angular/common';
import { Component, inject , ChangeDetectionStrategy } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { UserStore } from '../../../stores/user.store';
import { AccountService } from '../../../guest/account.service';
import { ThemeService } from '../../../core/services/theme.service';
import { ThemeToggleComponent } from '../theme-toggle/theme-toggle.component';
import { ROLES } from '../../../utilities/roles.const';

interface NavItem {
	link: string;
	name: string;
}

@Component({
    selector: 'app-header',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLinkActive, RouterLink, CommonModule, ThemeToggleComponent],
    templateUrl: './header.component.html',
    styleUrl: './header.component.scss'
})
export class HeaderComponent {
	readonly ROLES = ROLES;
	showAccountOption = false;
	showAIDropdownMenu = false;
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

	onShowAccountOption() {
		this.showAccountOption = true;
	}

	onHiddenAccountOption() {
		this.showAccountOption = false;
	}

	logout() {
		this.accountService.apiLogout().subscribe({ complete: () => {} });
		this.accountService.logout();
		this.router.navigateByUrl('');
	}
}
