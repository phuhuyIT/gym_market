import { Routes } from '@angular/router';
import { clientGuard } from './guards/client.guard';
import { agencyGuard } from './guards/agency.guard';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { AccessDeniedComponent } from './components/access-denied/access-denied.component';
import { CertificateVerifyComponent } from './certificate-verify/certificate-verify.component';

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
		path: 'admin',
		loadChildren: () => import('./admin/admin.routes').then(r => r.routes),
		canActivate: [adminGuard],
	},
		{
			path: 'chat',
			loadChildren: () => import('./chat/chat.routes').then(r => r.routes),
			canActivate: [authGuard],
		},
	    { path: 'certificate/verify/:verificationCode', component: CertificateVerifyComponent, title: 'Verify certificate' },
	    { path: 'access-denied', component: AccessDeniedComponent, title: 'Access denied' },
    { path: 'not-found', component: NotFoundComponent, title: 'Page not found' },
	{ path: '**', redirectTo: '/not-found' },
];
