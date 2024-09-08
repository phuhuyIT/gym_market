import { Routes } from '@angular/router';
import { SearchComponent } from './search/search.component';
import { CourseDetailsComponent } from './course-details/course-details.component';

export const routes: Routes = [
	{
		path: '',
		children: [
			{ path: 'search', component: SearchComponent, title: 'Tìm khóa học cho bạn' },
			{
				path: 'course-details',
				component: CourseDetailsComponent,
				title: 'Chi tiết khóa học',
			},
		],
	},
];
