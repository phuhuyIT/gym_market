import { Component, inject } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../../stores/error-modal.store';
import { AccountService } from '../../../services/account.service';

@Component({
	selector: 'app-signup',
	standalone: true,
	imports: [HeaderComponent, ReactiveFormsModule],
	templateUrl: './signup.component.html',
	styleUrl: './signup.component.scss',
})
export class SignupComponent {
	signUpForm!: FormGroup;
	submit = false;
	errorStore = inject(ErrorModalStore);

	constructor(
		private formBuilder: FormBuilder,
		private router: Router,
		private accountService: AccountService
	) {}

	ngOnInit() {
		if(this.accountService.isLogedIn() !== false) {
			this.router.navigateByUrl('/home')
		  } 
		  
		this.signUpForm = this.formBuilder.group({
			fullName: ['', [Validators.required]],
			email: ['', [Validators.required, Validators.email]],
			password: ['', [Validators.required]],
			confirmPassword: ['', [Validators.required]],
			role: ['Trainer'],
		});
	}

	// đánh dấu các trường không hợp lệ và hiển thị lỗi tương ứng
	addDirty(control: string) {
		this.signUpForm.controls[control].markAsDirty({ onlySelf: true });
	}

	removeDirty(control: string) {
		this.signUpForm.controls[control].markAsPristine({
			onlySelf: true,
		});
	}

	onSignUp() {
		this.submit = true;
		if (this.signUpForm.valid == false) {
			return;
		}

		if (
			this.signUpForm.controls['password'].value !==
			this.signUpForm.controls['confirmPassword'].value
		) {
			this.addDirty('confirmPassword');
			return;
		} else {
			this.signUpForm.controls['confirmPassword'].markAsPristine({
				onlySelf: true,
			});
		}

		console.log(this.signUpForm.value);
		this.accountService.signUp(this.signUpForm.value).subscribe({
			next: (res: any) => {
				console.log(res);
				this.router.navigateByUrl('/account/login');
			},
			error: err => {
				console.log(err.error.errors);
				patchState(this.errorStore, { isShow: true, errors: err.error.errors });
			},
		});
	}
}
