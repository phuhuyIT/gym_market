import { Routes } from '@angular/router';
import { AddCourseComponent } from './add-course/add-course.component';
import { UpdateCourseComponent } from './update-course/update-course.component';
import { CourseListComponent } from './course-list/course-list.component';
import { CourseAgencyComponent } from './course-agency.component';

export const routes: Routes = [
	{
		path: '',
        component: CourseAgencyComponent,
		children: [
			{ path: '', redirectTo: 'courses', pathMatch: 'full' },
			{ path: 'courses', component: CourseListComponent, title: 'courses' },
			{ path: 'add-course', component: AddCourseComponent, title: 'add course' },
			{ path: 'update-course/:id', component: UpdateCourseComponent, title: 'update course' },
		],
	},
];
