import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

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

	constructor(private formBuilder: FormBuilder) {}

	ngOnInit() {
		this.signUpForm = this.formBuilder.group({
			fullName: ['', [Validators.required]],
			email: ['', [Validators.required, Validators.email]],
			password: ['', [Validators.required]],
			confirmPassword: ['', [Validators.required]],
			role: ['PT'],
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
		}

		console.log(this.signUpForm.value);
	}
}
