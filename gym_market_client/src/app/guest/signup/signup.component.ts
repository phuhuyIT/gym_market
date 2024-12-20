import { Component, inject } from '@angular/core';
import { HeaderComponent } from '../../pages-client/components/header/header.component';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { TrainerSignup } from '../models/trainer-sign-up.model';
import { StudentSignup } from '../models/student-sign-up.model';
import { LoaderModalStore } from '../../stores/loader.store';
import { AccountService } from '../account.service';

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
	loaderStore = inject(LoaderModalStore);

	constructor(
		private formBuilder: FormBuilder,
		private router: Router,
		private accountService: AccountService
	) {}

	ngOnInit() {
		if (this.accountService.isLogedIn() !== false) {
			this.router.navigateByUrl('/home');
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

		patchState(this.loaderStore, { isShow: true });
		this.accountService.signUp(this.signUpForm.value).subscribe({
			next: (res: any) => {
				console.log(res); // res.userId
				if (this.signUpForm.controls['role'].value === 'Trainer') {
					this.trainerSignup(res);
				} else if (this.signUpForm.controls['role'].value === 'Student') {
					this.studentSignup(res);
				}
				patchState(this.loaderStore, { isShow: false });
			},
			error: err => {
				patchState(this.errorStore, { isShow: true, errors: err.error.errors });
				patchState(this.loaderStore, { isShow: false });
			},
		});
	}

	private trainerSignup(res: any) {
		const model: TrainerSignup = {
			bio: '',
			certification: '',
			createdAt: new Date(),
			email: this.signUpForm.controls['email'].value,
			experience: 0,
			name: this.signUpForm.controls['fullName'].value,
			password: this.signUpForm.controls['password'].value,
			profilePicture: 'https://cdn-icons-png.flaticon.com/512/236/236832.png',
			rating: 0,
			updatedAt: new Date(),
			trainerId: crypto.randomUUID(),
			userId: res.userId,
		};

		this.accountService.trainerSignup(model).subscribe({
			next: (res: any) => {
				patchState(this.loaderStore, { isShow: false });
				this.router.navigateByUrl('/login');
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				let result = [];
				for (const key in err.error.errors) {
					if (err.error.errors.hasOwnProperty(key)) {
						result.push(`${key}: ${err.error.errors[key][0]}\n`);
					}
				}
				patchState(this.errorStore, { isShow: true, errors: result });
				patchState(this.loaderStore, { isShow: false });
			},
		});
	}

	private studentSignup(res: any) {
		const model: StudentSignup = {
			createdAt: new Date(),
			email: this.signUpForm.controls['email'].value,
			name: this.signUpForm.controls['fullName'].value,
			password: this.signUpForm.controls['password'].value,
			profilePicture: 'https://cdn-icons-png.flaticon.com/512/236/236832.png',
			updatedAt: new Date(),
			healthStatus: '',
			studentId: crypto.randomUUID(),
			userId: res.userId,
		};

		this.accountService.studentSignup(model).subscribe({
			next: (res: any) => {
				patchState(this.loaderStore, { isShow: false });
				this.router.navigateByUrl('/login');
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				let result = [];
				for (const key in err.error.errors) {
					if (err.error.errors.hasOwnProperty(key)) {
						result.push(`${key}: ${err.error.errors[key][0]}\n`);
					}
				}
				patchState(this.errorStore, { isShow: true, errors: result });
				patchState(this.loaderStore, { isShow: false });
			},
		});
	}
}
