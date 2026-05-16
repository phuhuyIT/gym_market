import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router, RouterLink } from '@angular/router';
import { ToastService } from '../../shared/services/toast.service';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { ROLES } from '../../utilities/roles.const';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LoginResponse } from '../../core/models/auth.model';
import { CommonModule } from '@angular/common';
import { GmInputComponent, GmButtonComponent } from '../../shared';

@Component({
	selector: 'app-login',
	standalone: true,
	imports: [CommonModule, FormsModule, RouterLink, GmInputComponent, GmButtonComponent],
	templateUrl: './login.component.html',
	styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
	model = {
		email: '',
		password: '',
	};
	loading = false;
	toastService = inject(ToastService);
	loader = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);

	constructor(
		private router: Router,
		private accountService: AccountService
	) {}

	ngOnInit() {
		if (this.accountService.isLoggedIn()) {
			this.router.navigateByUrl('/home');
		}
	}

	onLogin() {
		if (!this.model.email || !this.model.password) {
			this.toastService.show('Please fill in all fields', 'error');
			return;
		}

		this.loading = true;
		patchState(this.loader, { isShow: true });
		
		this.accountService
			.login(this.model)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: (res: LoginResponse) => {
					this.loading = false;
					patchState(this.loader, { isShow: false });
					this.accountService.saveToken(res.token);
					this.accountService.checkLogin();
					const role = this.accountService.getRole();

					if (role === ROLES.TRAINER) {
						this.router.navigateByUrl('/agency');
						return;
					} else if (role === ROLES.STUDENT) {
						this.router.navigateByUrl('/client');
						return;
					}
					this.router.navigateByUrl('/access-denied');
				},
				error: err => {
					this.loading = false;
					patchState(this.loader, { isShow: false });
					const errors = err.error?.errors || ['Login failed'];
					this.toastService.show(errors.join(', '), 'error');
				},
			});
	}
}
