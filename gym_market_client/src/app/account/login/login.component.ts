import { Component, inject } from '@angular/core';
import {
	FormBuilder,
	FormGroup,
	ReactiveFormsModule,
	Validators,
} from '@angular/forms';
import { AccountService } from '../../services/account.service';
import { Router } from '@angular/router';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { ROLES } from '../../utilities/roles.const';

@Component({
	selector: 'app-login',
	standalone: true,
	imports: [ReactiveFormsModule],
	templateUrl: './login.component.html',
	styleUrl: './login.component.scss',
})
export class LoginComponent {
	signUpForm!: FormGroup;
	submit = false;
	errorModalStore = inject(ErrorModalStore);
	loader = inject(LoaderModalStore);

	constructor(
		private formBuilder: FormBuilder,
		private router: Router,
		private accountService: AccountService
	) {}

	ngOnInit() {
		if (this.accountService.isLogedIn() === true) {
			console.log(this.accountService.isLogedIn());

			this.router.navigateByUrl('/home');
		}

		this.signUpForm = this.formBuilder.group({
			email: ['', [Validators.email, Validators.required]],
			password: ['', [Validators.required]],
		});
	}

	// đánh dấu các trường không hợp lệ và hiển thị lỗi tương ứng
	validateAllFormFields(control: string) {
		this.signUpForm.controls[control].markAsDirty({ onlySelf: true });
	}

	onSignIn() {
		this.submit = true;
		if (this.signUpForm.valid == false) {
			return;
		}

		patchState(this.loader, { isShow: true });
		this.accountService.login(this.signUpForm.value).subscribe({
			next: (res: any) => {
				// console.log(res);
				patchState(this.loader, { isShow: false });
				this.accountService.saveToken(res.token);
                const role = this.accountService.getRole();

                if(role === ROLES.TRAINER) {
                    this.router.navigateByUrl('/agency');
                    return;
                } else if(role === ROLES.STUDENT) {
                    this.router.navigateByUrl('/client');
                    return;
                }
                this.router.navigateByUrl('/access-denied');
			},
			error: err => {
				patchState(this.loader, { isShow: false });
				patchState(this.errorModalStore, { errors: err.error.errors, isShow: true });
			},
		});
	}
}
