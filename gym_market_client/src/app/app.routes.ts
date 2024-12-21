import { Routes } from '@angular/router';
import { guestGuard } from './guards/guest.guard';
import { clientGuard } from './guards/client.guard';
import { agencyGuard } from './guards/agency.guard';
import { NotFoundComponent } from './components/access-denied/not-found.component';

export const routes: Routes = [
	{
		path: '',
		loadChildren: () => import('./guest/account.routes').then(r => r.routes),
		
	},
	{
		path: 'client',
		loadChildren: () => import('./pages-client/pages-client.routes').then(r => r.routes),
		canActivate: [clientGuard],
	},
	{
		path: 'agency',
		loadChildren: () => import('./page-agency/course-agency.routes').then(r => r.routes),
		canActivate: [agencyGuard],
	},
    {
		path: 'chat',
		loadChildren: () => import('./chat/chat.routes').then(r => r.routes),
	},
    { path: 'access-denied', component: NotFoundComponent, title: 'Access denied' },
	{ path: '**', redirectTo: '/guest/access-denied' },
];
