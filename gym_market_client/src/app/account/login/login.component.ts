import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [HeaderComponent, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  signUpForm!: FormGroup
  submit = false;

  constructor(private formBuilder: FormBuilder) {}

  ngOnInit() {
    this.signUpForm = this.formBuilder.group({
      email: ['', [Validators.email, Validators.required]],
      password: ['',[Validators.required]]
    })
  }

  // đánh dấu các trường không hợp lệ và hiển thị lỗi tương ứng
  validateAllFormFields(control: string) {
    this.signUpForm.controls[control].markAsDirty({ onlySelf: true })
  }

  onSignUp() {
    this.submit = true;
    if(this.signUpForm.valid == false) {

      return;
    }

    console.log(this.signUpForm.value)
  }
}
