import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import {
	FormBuilder,
	FormGroup,
	ReactiveFormsModule,
	Validators,
} from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';
import { ToastService } from '../../shared/services/toast.service';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { ROLES } from '../../utilities/roles.const';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LoginResponse } from '../../core/models/auth.model';

@Component({
	selector: 'app-login',
	standalone: true,
	imports: [ReactiveFormsModule],
	templateUrl: './login.component.html',
	styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
	signUpForm!: FormGroup;
	submit = false;
	toastService = inject(ToastService);
	loader = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);

	constructor(
		private formBuilder: FormBuilder,
		private router: Router,
		private accountService: AccountService
	) {}

	ngOnInit() {
		if (this.accountService.isLoggedIn()) {
			this.router.navigateByUrl('/home');
		}

		this.signUpForm = this.formBuilder.group({
			email: ['', [Validators.email, Validators.required]],
			password: ['', [Validators.required]],
		});
	}

	validateAllFormFields(control: string) {
		this.signUpForm.controls[control].markAsDirty({ onlySelf: true });
	}

	onSignIn() {
		this.submit = true;
		if (this.signUpForm.valid === false) {
			return;
		}

		patchState(this.loader, { isShow: true });
		this.accountService
			.login(this.signUpForm.value)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: (res: LoginResponse) => {
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
					patchState(this.loader, { isShow: false });
					const errors = err.error?.errors || ['Login failed'];
					this.toastService.show(errors.join(', '), 'error');
				},
			});
	}
}
