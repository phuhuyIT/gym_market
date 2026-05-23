import { Component, DestroyRef, inject, OnInit, AfterViewInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../account.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ToastService } from '../../shared/services/toast.service';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LoginResponse } from '../../core/models/auth.model';

import { GmInputComponent, GmButtonComponent } from '../../shared';
import { environment } from '../../../environments/environment.development';

@Component({
    selector: 'app-login',
    imports: [FormsModule, RouterLink, GmInputComponent, GmButtonComponent],
    templateUrl: './login.component.html',
    styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit, AfterViewInit {
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
		private route: ActivatedRoute,
		private accountService: AccountService
	) {}

	ngOnInit() {
		if (this.accountService.isLoggedIn()) {
			this.navigateAfterLogin();
		}
	}

	private navigateAfterLogin() {
		const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
		this.router.navigateByUrl(
			this.accountService.isSafeReturnUrl(returnUrl) ? returnUrl : this.accountService.defaultLandingUrl()
		);
	}

	ngAfterViewInit() {
		this.accountService.loadGoogleLibrary().then(() => {
			this.initGoogleButton();
		});
	}

	private initGoogleButton() {
		const google = (window as any).google;
		if (google?.accounts?.id) {
			google.accounts.id.initialize({
				client_id: environment.googleClientId,
				callback: this.handleGoogleCredential.bind(this),
			});
			const element = document.getElementById('google-login-btn');
			if (element) {
				google.accounts.id.renderButton(element, {
					theme: 'outline',
					size: 'large',
					width: 320,
					text: 'signin_with',
					shape: 'pill',
				});
			}
		}
	}

	private handleGoogleCredential(response: any) {
		if (!response.credential) return;

		this.loading = true;
		patchState(this.loader, { isShow: true });

		this.accountService
			.googleLogin(response.credential)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: (res: LoginResponse) => {
					this.loading = false;
					patchState(this.loader, { isShow: false });
					this.accountService.saveToken(res.token);
					this.accountService.checkLogin();
					this.navigateAfterLogin();
				},
				error: err => {
					this.loading = false;
					patchState(this.loader, { isShow: false });
					const errors = err.error?.errors || ['Google login failed'];
					this.toastService.show(errors.join(', '), 'error');
				},
			});
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
					this.navigateAfterLogin();
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
