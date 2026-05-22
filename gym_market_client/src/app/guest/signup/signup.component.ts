import { Component, DestroyRef, inject, OnInit, AfterViewInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { patchState } from '@ngrx/signals';
import { ToastService } from '../../shared/services/toast.service';
import { TrainerSignup } from '../models/trainer-sign-up.model';
import { StudentSignup } from '../models/student-sign-up.model';
import { LoaderModalStore } from '../../stores/loader.store';
import { AccountService } from '../account.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { SignupResponse, LoginResponse } from '../../core/models/auth.model';
import { CommonModule } from '@angular/common';
import { GmInputComponent, GmButtonComponent } from '../../shared';
import { environment } from '../../../environments/environment.development';
import { ROLES } from '../../utilities/roles.const';

@Component({
    selector: 'app-signup',
    imports: [CommonModule, FormsModule, RouterLink, GmInputComponent, GmButtonComponent],
    templateUrl: './signup.component.html',
    styleUrl: './signup.component.scss'
})
export class SignupComponent implements OnInit, AfterViewInit {
	readonly ROLES = ROLES;
	model = {
		fullName: '',
		email: '',
		password: '',
		confirmPassword: '',
		role: '',
		// Trainer extra fields
		bio: '',
		specialization: '',
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
		const google = (window as any).google;
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

	private handleGoogleCredential(response: any) {
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
			role: this.model.role
		};

		this.accountService
			.signUp(signupData)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: (res: SignupResponse) => {
					if (this.model.role === ROLES.TRAINER) {
						this.trainerSignup(res);
					} else if (this.model.role === ROLES.STUDENT) {
						this.studentSignup(res);
					}
				},
				error: err => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					const errors = err.error?.errors || ['Signup failed'];
					this.toastService.show(errors instanceof Array ? errors.join(', ') : errors, 'error');
				},
			});
	}

	private trainerSignup(res: SignupResponse) {
		const trainerModel: TrainerSignup = {
			bio: this.model.bio,
			certification: this.model.specialization,
			createdAt: new Date(),
			email: this.model.email,
			experience: Number(this.model.experience),
			name: this.model.fullName,
			password: this.model.password,
			profilePicture: 'https://cdn-icons-png.flaticon.com/512/236/236832.png',
			rating: 0,
			updatedAt: new Date(),
			trainerId: crypto.randomUUID(),
			userId: res.userId,
		};

		this.accountService
			.trainerSignup(trainerModel)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					this.router.navigateByUrl('/login');
				},
				error: err => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Trainer signup details failed', 'error');
				},
			});
	}

	private studentSignup(res: SignupResponse) {
		const studentModel: StudentSignup = {
			createdAt: new Date(),
			email: this.model.email,
			name: this.model.fullName,
			password: this.model.password,
			profilePicture: 'https://cdn-icons-png.flaticon.com/512/236/236832.png',
			updatedAt: new Date(),
			healthStatus: this.model.healthStatus,
			studentId: crypto.randomUUID(),
			userId: res.userId,
		};

		this.accountService
			.studentSignup(studentModel)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					this.router.navigateByUrl('/login');
				},
				error: err => {
					this.loading = false;
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Student signup details failed', 'error');
				},
			});
	}
}
