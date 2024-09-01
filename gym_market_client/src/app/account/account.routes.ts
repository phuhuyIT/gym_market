import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { SignupComponent } from './signup/signup.component';

export const routes: Routes = [
	{
		path: '',
		children: [
			{ path: 'login', component: LoginComponent, title: 'Login' },
			{ path: 'sign-up', component: SignupComponent, title: 'Sign up' },
			{ path: '**', redirectTo: 'login', pathMatch: 'full' },
		],
	},
];
