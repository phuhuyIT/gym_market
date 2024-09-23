import { Routes } from '@angular/router';
import { SearchComponent } from './search/search.component';
import { TrainderDetailsComponent } from './trainder-details/trainder-details.component';

export const routes: Routes = [
	{
		path: '',
		children: [
			{
				path: 'search',
				title: 'Find your trainers',
				component: SearchComponent,
			},
			{
				path: 'trainer-details/:id',
				title: 'Details',
				component: TrainderDetailsComponent,
			},
		],
	},
];
