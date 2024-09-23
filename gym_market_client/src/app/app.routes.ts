import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { AccessDeniedComponent } from './pages/access-denied/access-denied.component';

export const routes: Routes = [
	{ path: 'home', component: HomeComponent, title: 'Home' },
	{ path: '', redirectTo: 'home', pathMatch: 'full' },
	{
		path: 'account',
		loadChildren: () => import('./pages/account/account.routes').then(r => r.routes),
	},
	{
		path: 'courses',
		loadChildren: () => import('./pages/courses/course.routes').then(r => r.routes),
	},
	{
		path: 'trainers',
		loadChildren: () => import('./pages/trainers/trainers.routes').then(r => r.routes),
	},
	{ path: 'access-denied', component: AccessDeniedComponent, title: 'Access denied' },
	{ path: '**', redirectTo: 'home' },
];
