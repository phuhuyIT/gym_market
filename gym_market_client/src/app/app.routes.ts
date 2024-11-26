import { Routes } from '@angular/router';

export const routes: Routes = [
	
	{
		path: 'guest',
		loadChildren: () => import('./guest/guest.routes').then(r => r.routes),
	},
	{
		path: 'account',
		loadChildren: () => import('./pages-client/account/account.routes').then(r => r.routes),
	},
	
	
	{ path: '**', redirectTo: '/guest/access-denied' },
];
