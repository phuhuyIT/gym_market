import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { PagesClientComponent } from './pages-client.component';
import { YourProfileComponent } from './your-profile/your-profile.component';

export const routes: Routes = [
	{
		path: '',
        component: PagesClientComponent,
		children: [
			{ path: '', redirectTo: 'home-client', pathMatch: 'full' },
			{ path: 'home-client', component: HomeComponent, title: 'Home' },
			{ path: 'your-profile', component: YourProfileComponent, title: 'Your profile' },
            
		],
	},
];
