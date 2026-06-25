import { Routes } from '@angular/router';
import { AdminComponent } from './admin.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { ManageCoursesComponent } from '../page-agency/manage-courses/manage-courses.component';
import { ManageStudentsComponent } from '../page-agency/manage-students/manage-students.component';
import { PaymentsComponent } from '../page-agency/payments/payments.component';
import { ManageNutritionComponent } from '../page-agency/manage-nutrition/manage-nutrition.component';
import { AddCourseComponent } from '../page-agency/add-course/add-course.component';
import { UpdateCourseComponent } from '../page-agency/update-course/update-course.component';
import { CourseMaterialComponent } from '../page-agency/course-material/course-material.component';
import { CourseOptionListComponent } from '../page-agency/course-option-list/course-option-list.component';
import { AccountSettingsComponent } from '../account-settings/account-settings.component';

export const routes: Routes = [
	{
		path: '',
		component: AdminComponent,
		children: [
			{ path: '', redirectTo: 'dashboard', pathMatch: 'full' },
			{ path: 'dashboard', component: AdminDashboardComponent, title: 'Admin Dashboard' },
			{ path: 'courses', component: ManageCoursesComponent, title: 'Courses' },
			{ path: 'students', component: ManageStudentsComponent, title: 'Students' },
			{ path: 'payments', component: PaymentsComponent, title: 'Payments' },
			{ path: 'nutrition', component: ManageNutritionComponent, title: 'Food Database' },
			{ path: 'add-course', component: AddCourseComponent, title: 'Add Course' },
			{ path: 'update-course/:id', component: UpdateCourseComponent, title: 'Edit Course' },
			{ path: 'course-materials/:courseId', component: CourseMaterialComponent, title: 'Course Materials' },
			{ path: 'course-option-list', component: CourseOptionListComponent, title: 'Course Options' },
			{ path: 'account-settings', component: AccountSettingsComponent, title: 'Account Settings' },
		],
	},
];
