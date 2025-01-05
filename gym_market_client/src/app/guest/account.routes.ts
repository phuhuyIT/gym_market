import { Routes } from '@angular/router';
import { GuestComponent } from './account.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { SignupComponent } from './signup/signup.component';
import { CheckBmiComponent } from './check-bmi/check-bmi.component';
import { guestGuard } from '../guards/guest.guard';
import { PredicyBodyFastByImageComponent } from './predicy-body-fast-by-image/predicy-body-fast-by-image.component';

export const routes: Routes = [
	{
		path: '',
		component: GuestComponent,
		children: [
            { path: '', redirectTo: 'home', pathMatch: 'full' },
			{ path: 'home', component: HomeComponent, title: 'guest', },
			{ path: 'login', component: LoginComponent, title: 'Login',canActivate: [guestGuard], },
			{ path: 'sign-up', component: SignupComponent, title: 'Sign up',canActivate: [guestGuard], },
			{ path: 'check-bmi', component: CheckBmiComponent, title: 'Check BMI' },
			{ path: 'predict-body-fat', component: PredicyBodyFastByImageComponent, title: 'Predict Body Fat' },
		],
	},
];
