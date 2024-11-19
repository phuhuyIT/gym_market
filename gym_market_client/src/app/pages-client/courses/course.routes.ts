import { Routes } from '@angular/router';
import { SearchComponent } from './search/search.component';
import { CourseDetailsComponent } from './course-details/course-details.component';
import { MyCoursesComponent } from './my-courses/my-courses.component';

export const routes: Routes = [
	{
		path: '',
		children: [
			{ path: 'search', component: SearchComponent, title: 'Tìm khóa học cho bạn' },
			{
				path: 'course-details/:id',
				component: CourseDetailsComponent,
				title: 'Chi tiết khóa học',
			},
			{
				path: 'my-courses/:type',
				component: MyCoursesComponent,
				title: 'Khóa học của bạn',
			},
		],
	},
];
