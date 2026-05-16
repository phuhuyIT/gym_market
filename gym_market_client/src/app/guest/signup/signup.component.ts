import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { patchState } from '@ngrx/signals';
import { ToastService } from '../../shared/services/toast.service';
import { TrainerSignup } from '../models/trainer-sign-up.model';
import { StudentSignup } from '../models/student-sign-up.model';
import { LoaderModalStore } from '../../stores/loader.store';
import { AccountService } from '../account.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { SignupResponse } from '../../core/models/auth.model';
import { CommonModule } from '@angular/common';
import { GmInputComponent, GmButtonComponent } from '../../shared';

@Component({
	selector: 'app-signup',
	standalone: true,
	imports: [CommonModule, FormsModule, RouterLink, GmInputComponent, GmButtonComponent],
	templateUrl: './signup.component.html',
	styleUrl: './signup.component.scss',
})
export class SignupComponent implements OnInit {
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

	selectRole(role: string) {
		this.model.role = role;
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
					if (this.model.role === 'Trainer') {
						this.trainerSignup(res);
					} else if (this.model.role === 'Student') {
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
