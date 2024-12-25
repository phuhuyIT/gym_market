import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { UserStore } from '../../../stores/user.store';
import { AccountService } from '../../account.service';

@Component({
	selector: 'app-header',
	standalone: true,
	imports: [CommonModule, RouterLink],
	templateUrl: './header.component.html',
	styleUrl: './header.component.scss',
})
export class HeaderComponent {
	showAccountOption = false;
	showAIDropdownMenu: boolean = false;
	user = inject(UserStore);

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

	constructor(private router: Router, private accountService: AccountService) {}

	onShowAccountOption() {
		this.showAccountOption = true;
	}

	onHiddenAccountOption() {
		this.showAccountOption = false;
	}

	onClick() {
		this.router.navigateByUrl('/');
	}

    logout() {
		this.accountService.logout();
		this.router.navigateByUrl('');
	}
}
