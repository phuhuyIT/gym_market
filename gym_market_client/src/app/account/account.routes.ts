import { Routes } from '@angular/router';
import { GuestComponent } from './account.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { SignupComponent } from './signup/signup.component';

export const routes: Routes = [
	{
		path: '',
		component: GuestComponent,
		children: [
            { path: '', redirectTo: 'home', pathMatch: 'full' },
			{ path: 'home', component: HomeComponent, title: 'guest' },
			{ path: 'login', component: LoginComponent, title: 'Login' },
			{ path: 'sign-up', component: SignupComponent, title: 'Sign up' },
		],
	},
];
