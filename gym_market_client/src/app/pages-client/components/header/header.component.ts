import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { UserStore } from '../../../stores/user.store';
import { AccountService } from '../../account/account.service';

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

	constructor(private accountService: AccountService, private router: Router) {}

	onShowAccountOption() {
		this.showAccountOption = true;
	}

	onHiddenAccountOption() {
		this.showAccountOption = false;
	}

	logout() {
		this.accountService.logout();
		this.router.navigateByUrl('/account/login');
	}
}
