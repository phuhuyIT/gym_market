import { Routes } from '@angular/router';
import { AdminComponent } from './admin.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { ManageStudentsComponent } from '../page-agency/manage-students/manage-students.component';
import { PaymentsComponent } from '../page-agency/payments/payments.component';
import { ManageNutritionComponent } from '../page-agency/manage-nutrition/manage-nutrition.component';
import { AddCourseComponent } from '../page-agency/add-course/add-course.component';
import { UpdateCourseComponent } from '../page-agency/update-course/update-course.component';
import { CourseMaterialComponent } from '../page-agency/course-material/course-material.component';
import { AssignmentsComponent } from '../page-agency/assignments/assignments.component';
import { GradebookComponent } from '../page-agency/gradebook/gradebook.component';
import { CourseDiscussionsComponent } from '../page-agency/course-discussions/course-discussions.component';
import { CourseAnnouncementsComponent } from '../page-agency/course-announcements/course-announcements.component';
import { CourseOptionListComponent } from '../page-agency/course-option-list/course-option-list.component';
import { AccountSettingsComponent } from '../account-settings/account-settings.component';
import { AdminUsersComponent } from './admin-users/admin-users.component';
import { AdminCoursesComponent } from './admin-courses/admin-courses.component';
import { MembershipsComponent } from '../page-agency/memberships/memberships.component';
import { ClassScheduleComponent } from '../page-agency/class-schedule/class-schedule.component';
import { WorkoutPlansComponent } from '../page-agency/workout-plans/workout-plans.component';
import { ProgressReviewComponent } from '../page-agency/progress-review/progress-review.component';
import { NotificationCenterComponent } from '../shared/components/notification-center/notification-center.component';
import { AdminNotificationsComponent } from './admin-notifications/admin-notifications.component';

export const routes: Routes = [
	{
		path: '',
		component: AdminComponent,
		children: [
				{ path: '', redirectTo: 'dashboard', pathMatch: 'full' },
				{ path: 'dashboard', component: AdminDashboardComponent, title: 'Admin Dashboard' },
				{ path: 'users', component: AdminUsersComponent, title: 'Users' },
				{ path: 'courses', component: AdminCoursesComponent, title: 'Course Review' },
				{ path: 'students', component: ManageStudentsComponent, title: 'Students' },
				{ path: 'payments', component: PaymentsComponent, title: 'Payments' },
				{ path: 'memberships', component: MembershipsComponent, title: 'Memberships' },
				{ path: 'classes', component: ClassScheduleComponent, title: 'Class Schedule' },
				{ path: 'workouts', component: WorkoutPlansComponent, title: 'Workout Plans' },
				{ path: 'progress', component: ProgressReviewComponent, title: 'Progress Review' },
				{ path: 'notifications', component: NotificationCenterComponent, title: 'Notifications' },
				{ path: 'notification-ops', component: AdminNotificationsComponent, title: 'Notification Operations' },
				{ path: 'nutrition', component: ManageNutritionComponent, title: 'Food Database' },
			{ path: 'add-course', component: AddCourseComponent, title: 'Add Course' },
			{ path: 'update-course/:id', component: UpdateCourseComponent, title: 'Edit Course' },
			{ path: 'course-materials/:courseId', component: CourseMaterialComponent, title: 'Course Materials' },
				{ path: 'assignments/:courseId', component: AssignmentsComponent, title: 'Assignments' },
				{ path: 'announcements/:courseId', component: CourseAnnouncementsComponent, title: 'Course announcements' },
				{ path: 'discussions/:courseId', component: CourseDiscussionsComponent, title: 'Course Q&A' },
				{ path: 'gradebook/:courseId', component: GradebookComponent, title: 'Gradebook' },
			{ path: 'course-option-list', component: CourseOptionListComponent, title: 'Course Options' },
			{ path: 'account-settings', component: AccountSettingsComponent, title: 'Account Settings' },
		],
	},
];
