import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { UserStore } from '../../../stores/user.store';
import { AccountService } from '../../../guest/account.service';

@Component({
	selector: 'app-header',
	standalone: true,
	imports: [RouterLinkActive, RouterLink, CommonModule],
	templateUrl: './header.component.html',
	styleUrl: './header.component.scss',
})
export class HeaderComponent {
	showAccountOption = false;
	userStore = inject(UserStore);

	navbarItems: any = [
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

   
	aiMenu = [
		{
			name: 'Predict body fat',
			link: '/check-bmi',
		},
        {
			name: 'Predict body fat by Image',
			link: '/predict-body-fat',
		},
	];
	showAIDropdownMenu: boolean = false;

	constructor(private accountService: AccountService, private router: Router) {}

	onShowAccountOption() {
		this.showAccountOption = true;
	}

	onHiddenAccountOption() {
		this.showAccountOption = false;
	}

	logout() {
		this.accountService.logout();
		this.router.navigateByUrl('');
	}
}
