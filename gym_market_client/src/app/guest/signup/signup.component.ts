import { Component, DestroyRef, inject, OnInit, AfterViewInit , ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { patchState } from '@ngrx/signals';
import { ToastService } from '../../shared/services/toast.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { AccountService } from '../account.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LoginResponse } from '../../core/models/auth.model';

import { GmInputComponent, GmButtonComponent } from '../../shared';
import { environment } from '../../../environments/environment.development';
import { ROLES } from '../../utilities/roles.const';

@Component({
    selector: 'app-signup',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [FormsModule, RouterLink, GmInputComponent, GmButtonComponent],
    templateUrl: './signup.component.html',
    styleUrl: './signup.component.scss'
})
export class SignupComponent implements OnInit, AfterViewInit {
	readonly ROLES = ROLES;
	readonly trainerCategories = ['Yoga', 'Cardio', 'Strength', 'Crossfit'];
	model = {
		fullName: '',
		email: '',
		password: '',
		confirmPassword: '',
		role: '',
		// Trainer extra fields
		bio: '',
		specialization: '',
		category: '',
		experience: 0,
		// Student extra fields
		healthStatus: '',
	};
	
	loading = false;
	toastService = inject(ToastService);
	loaderStore = inject(LoaderModalStore);
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

	ngAfterViewInit() {
		this.accountService.loadGoogleLibrary();
	}

	selectRole(role: string) {
		this.model.role = role;

		// Render the Google button after Angular renders the form container
		setTimeout(() => {
			this.initGoogleButton();
		}, 50);
	}

	private initGoogleButton() {
		const google = window.google;
		if (google?.accounts?.id) {
			google.accounts.id.initialize({
				client_id: environment.googleClientId,
				callback: this.handleGoogleCredential.bind(this),
			});
			const element = document.getElementById('google-signup-btn');
			if (element) {
				google.accounts.id.renderButton(element, {
					theme: 'outline',
					size: 'large',
					width: 320,
					text: 'signup_with',
					shape: 'pill',
				});
			}
		}
	}

	private handleGoogleCredential(response: GoogleCredentialResponse) {
		if (!response.credential) return;

		this.loading = true;
		patchState(this.loaderStore, { isShow: true });

		this.accountService
			.googleLogin(response.credential, this.model.role)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: (res: LoginResponse) => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					this.accountService.saveToken(res.token);
					this.accountService.checkLogin();
					
					if (this.model.role === ROLES.TRAINER) {
						this.router.navigateByUrl('/agency');
					} else {
						this.router.navigateByUrl('/client');
					}
				},
				error: err => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					const errors = err.error?.errors || ['Google signup failed'];
					this.toastService.show(errors instanceof Array ? errors.join(', ') : errors, 'error');
				},
			});
	}

	onSignUp() {
		if (!this.model.fullName || !this.model.email || !this.model.password) {
			this.toastService.show('Please fill in all basic fields', 'error');
			return;
		}

		if (this.model.password !== this.model.confirmPassword) {
			this.toastService.show('Passwords do not match', 'error');
			return;
		}

		this.loading = true;
		patchState(this.loaderStore, { isShow: true });

		const signupData = {
			fullName: this.model.fullName,
			email: this.model.email,
			password: this.model.password,
			confirmPassword: this.model.confirmPassword,
			role: this.model.role,
			healthStatus: this.model.healthStatus,
			certification: this.model.specialization,
			category: this.model.category,
			bio: this.model.bio,
			experience: Number(this.model.experience) || 0,
		};

		this.accountService
			.signUp(signupData)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Account created. Please check your email to confirm your account.', 'success');
					this.router.navigateByUrl('/login');
				},
				error: err => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					const errors = err.error?.errors || ['Signup failed'];
					this.toastService.show(errors instanceof Array ? errors.join(', ') : errors, 'error');
				},
			});
	}
}
