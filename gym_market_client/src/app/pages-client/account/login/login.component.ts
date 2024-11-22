import { Component, inject } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import {
	FormBuilder,
	FormControl,
	FormGroup,
	ReactiveFormsModule,
	Validators,
} from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';
import { ErrorModalStore } from '../../../stores/error-modal.store';
import { patchState } from '@ngrx/signals';

@Component({
	selector: 'app-login',
	standalone: true,
	imports: [HeaderComponent, ReactiveFormsModule],
	templateUrl: './login.component.html',
	styleUrl: './login.component.scss',
})
export class LoginComponent {
	signUpForm!: FormGroup;
	submit = false;
	errorModalStore = inject(ErrorModalStore);

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

		console.log(this.signUpForm.value);
		this.accountService.login(this.signUpForm.value).subscribe({
			next: (res: any) => {
				// console.log(res);
				this.accountService.saveToken(res.token);
				this.router.navigateByUrl('/home');
			},
			error: err => {
				patchState(this.errorModalStore, { errors: err.error.errors, isShow: true });
				// console.log(err.error.errors);
				// console.log(err);
			},
		});
	}
}
