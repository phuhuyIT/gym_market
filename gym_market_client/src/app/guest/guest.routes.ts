import { Routes } from '@angular/router';
import { GuestComponent } from './guest.component';
import { HomeComponent } from './home/home.component';
import { AccessDeniedComponent } from './access-denied/access-denied.component';
import { LoginComponent } from './login/login.component';
import { SignupComponent } from './signup/signup.component';

export const routes: Routes = [
	{
		path: '',
		component: GuestComponent,
		children: [
			{ path: 'home', component: HomeComponent, title: 'guest' },
			{ path: 'access-denied', component: AccessDeniedComponent, title: 'Access denied' },
			{ path: 'login', component: LoginComponent, title: 'Login' },
			{ path: 'sign-up', component: SignupComponent, title: 'Sign up' },
		],
	},
];
