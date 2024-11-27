import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';

export const routes: Routes = [
	{
		path: '',
		children: [
			{ path: '', redirectTo: 'home-client', pathMatch: 'full' },
			{ path: 'home-client', component: HomeComponent, title: 'home' },
		],
	},
];
