import { Routes } from '@angular/router';
import { AddCourseComponent } from './add-course/add-course.component';
import { UpdateCourseComponent } from './update-course/update-course.component';
import { CourseMaterialComponent } from './course-material/course-material.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CourseAgencyComponent } from './course-agency.component';
import { CourseOptionListComponent } from './course-option-list/course-option-list.component';
import { YourProfileComponent } from './your-profile/your-profile.component';
import { UpdateProfileComponent } from './update-profile/update-profile.component';
import { AccountSettingsComponent } from '../account-settings/account-settings.component';
import { ChatListComponent } from '../chat/chat-list/chat-list.component';
import { ManageCoursesComponent } from './manage-courses/manage-courses.component';
import { ManageStudentsComponent } from './manage-students/manage-students.component';
import { PaymentsComponent } from './payments/payments.component';
import { ManageNutritionComponent } from './manage-nutrition/manage-nutrition.component';

export const routes: Routes = [
	{
		path: '',
        component: CourseAgencyComponent,
		children: [
			{ path: '', redirectTo: 'dashboard', pathMatch: 'full' },
			{ path: 'dashboard', component: DashboardComponent, title: 'Tổng quan' },
			{ path: 'courses', component: ManageCoursesComponent, title: 'Quản lý khóa học' },
				{ path: 'students', component: ManageStudentsComponent, title: 'Quản lý học viên' },
				{ path: 'payments', component: PaymentsComponent, title: 'Payments' },
				{ path: 'nutrition', component: ManageNutritionComponent, title: 'Food Database' },
				{ path: 'add-course', component: AddCourseComponent, title: 'Thêm course' },
			{ path: 'update-course/:id', component: UpdateCourseComponent, title: 'Cập nhật course' },
			{ path: 'course-materials/:courseId', component: CourseMaterialComponent, title: 'Tài liệu khóa học' },
			{ path: 'course-option-list', component: CourseOptionListComponent, title: 'Danh sách option của course' },
			{ path: 'your-profile', component: YourProfileComponent, title: 'Thông tin cá nhân' },
			{ path: 'edit-profile', component: UpdateProfileComponent, title: 'Chỉnh sửa hồ sơ' },
			{ path: 'account-settings', component: AccountSettingsComponent, title: 'Account Settings' },
			{ path: 'chat', component: ChatListComponent, title: 'Community Chat' },
		],
	},
];
