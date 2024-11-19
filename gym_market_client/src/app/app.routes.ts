import { Routes } from '@angular/router';
import { HomeComponent } from './pages-client/home/home.component';
import { AccessDeniedComponent } from './pages-client/access-denied/access-denied.component';

export const routes: Routes = [
	{ path: 'home', component: HomeComponent, title: 'Home' },
	{ path: '', redirectTo: 'home', pathMatch: 'full' },
	{
		path: 'account',
		loadChildren: () => import('./pages-client/account/account.routes').then(r => r.routes),
	},
	{
		path: 'courses',
		loadChildren: () => import('./pages-client/courses/course.routes').then(r => r.routes),
	},
	{
		path: 'trainers',
		loadChildren: () => import('./pages-client/trainers/trainers.routes').then(r => r.routes),
	},
    {
		path: 'course-agency',
		loadChildren: () => import('./page-agency/course-agency.routes').then(r => r.routes),
	},
	{ path: 'access-denied', component: AccessDeniedComponent, title: 'Access denied' },
	{ path: '**', redirectTo: 'home' },
];
