import { Routes } from '@angular/router';
import { AddCourseComponent } from './add-course/add-course.component';
import { UpdateCourseComponent } from './update-course/update-course.component';
import { CourseListComponent } from './course-list/course-list.component';
import { CourseAgencyComponent } from './course-agency.component';
import { CourseOptionListComponent } from './course-option-list/course-option-list.component';
import { YourProfileComponent } from './your-profile/your-profile.component';
import { UpdateProfileComponent } from './update-profile/update-profile.component';

export const routes: Routes = [
	{
		path: '',
        component: CourseAgencyComponent,
		children: [
			{ path: '', redirectTo: 'courses', pathMatch: 'full' },
			{ path: 'courses', component: CourseListComponent, title: 'Danh sách khóa huấn luyện' },
			{ path: 'add-course', component: AddCourseComponent, title: 'Thêm course' },
			{ path: 'update-course/:id', component: UpdateCourseComponent, title: 'Cập nhật course' },
			{ path: 'course-option-list', component: CourseOptionListComponent, title: 'Danh sách option của course' },
			{ path: 'your-profile', component: YourProfileComponent, title: 'Thông tin cá nhân' },
			{ path: 'edit-profile', component: UpdateProfileComponent, title: 'Chỉnh sửa hồ sơ' },
		],
	},
];
