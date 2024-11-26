import { Routes } from '@angular/router';
import { LoginComponent } from '../../guest/login/login.component';
import { SignupComponent } from '../../guest/signup/signup.component';
import { ProfileComponent } from './profile/profile.component';

export const routes: Routes = [
	{
		path: '',
		children: [
			{ path: 'login', component: LoginComponent, title: 'Login' },
			{ path: 'sign-up', component: SignupComponent, title: 'Sign up' },
			{ path: 'profile', component: ProfileComponent, title: 'Profile' },
			{ path: '**', redirectTo: 'login', pathMatch: 'full' },
		],
	},
];
