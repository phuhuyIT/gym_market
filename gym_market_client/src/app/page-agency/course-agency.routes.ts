import { Routes } from '@angular/router';
import { AddCourseComponent } from './add-course/add-course.component';
import { UpdateCourseComponent } from './update-course/update-course.component';
import { CourseMaterialComponent } from './course-material/course-material.component';
import { AssignmentsComponent } from './assignments/assignments.component';
import { GradebookComponent } from './gradebook/gradebook.component';
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
import { MembershipsComponent } from './memberships/memberships.component';
import { ClassScheduleComponent } from './class-schedule/class-schedule.component';
import { WorkoutPlansComponent } from './workout-plans/workout-plans.component';
import { ProgressReviewComponent } from './progress-review/progress-review.component';
import { NotificationCenterComponent } from '../shared/components/notification-center/notification-center.component';

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
				{ path: 'memberships', component: MembershipsComponent, title: 'Memberships' },
				{ path: 'classes', component: ClassScheduleComponent, title: 'Class Schedule' },
					{ path: 'workouts', component: WorkoutPlansComponent, title: 'Workout Plans' },
					{ path: 'progress', component: ProgressReviewComponent, title: 'Progress Review' },
					{ path: 'notifications', component: NotificationCenterComponent, title: 'Notifications' },
					{ path: 'nutrition', component: ManageNutritionComponent, title: 'Food Database' },
				{ path: 'add-course', component: AddCourseComponent, title: 'Thêm course' },
			{ path: 'update-course/:id', component: UpdateCourseComponent, title: 'Cập nhật course' },
			{ path: 'course-materials/:courseId', component: CourseMaterialComponent, title: 'Tài liệu khóa học' },
			{ path: 'assignments/:courseId', component: AssignmentsComponent, title: 'Assignments' },
			{ path: 'gradebook/:courseId', component: GradebookComponent, title: 'Gradebook' },
			{ path: 'course-option-list', component: CourseOptionListComponent, title: 'Danh sách option của course' },
			{ path: 'your-profile', component: YourProfileComponent, title: 'Thông tin cá nhân' },
			{ path: 'edit-profile', component: UpdateProfileComponent, title: 'Chỉnh sửa hồ sơ' },
			{ path: 'account-settings', component: AccountSettingsComponent, title: 'Account Settings' },
			{ path: 'chat', component: ChatListComponent, title: 'Community Chat' },
		],
	},
];
