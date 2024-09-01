import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';

export const routes: Routes = [
	{ path: 'home', component: HomeComponent, title: 'Home' },
	{ path: '', redirectTo: 'home', pathMatch: 'full' },
	{ path: 'account', loadChildren: () => import('./account/account.routes').then(r => r.routes) },
	{ path: '**', redirectTo: 'home' },
];
