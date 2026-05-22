import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { UserService } from '../../user/user.service';
import { UserStore } from '../../stores/user.store';
import { StudentService } from '../student.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Student } from '../../core/models/student.model';
import { UserInfo, UserInfoResponse } from '../../core/models/auth.model';
import { CommonModule } from '@angular/common';
import { GmCardComponent, GmButtonComponent } from '../../shared';
import { patchState } from '@ngrx/signals';

@Component({
    selector: 'app-your-profile',
    imports: [RouterLink, CommonModule, GmCardComponent, GmButtonComponent],
    templateUrl: './your-profile.component.html',
    styleUrl: './your-profile.component.scss'
})
export class YourProfileComponent implements OnInit {
	userStore = inject(UserStore);
	private destroyRef = inject(DestroyRef);
	studentInfo: Student | null = null;
	userInfo: UserInfo | null = null;
	activeTab = 'OVERVIEW';

	constructor(
		private studentService: StudentService,
		private router: Router,
		private userService: UserService
	) {}

	ngOnInit() {
		this.getUserInfo();
		this.getStudentInfo();
	}

	private getUserInfo() {
		const userId = this.userStore.id();
		if (userId === null) return;

		this.userService
			.getUserInfo(userId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: (res: UserInfoResponse) => {
					this.userInfo = res.userInfo;
				},
				error: () => {
					this.router.navigateByUrl('/login');
				},
			});
	}

	private getStudentInfo() {
		const studentId = this.userStore.studentId();
		if (studentId) {
			this.studentService
				.getStudentInfo(studentId)
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: res => {
						this.studentInfo = res;
					},
					error: () => {
						this.router.navigateByUrl('/login');
					},
				});
			return;
		}

		const userId = this.userStore.id();
		if (!userId) return;

		this.studentService
			.getStudentInfoByUserId(userId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.studentInfo = res;
					if (res.studentId) {
						patchState(this.userStore, { studentId: res.studentId });
					}
				},
				error: () => {
					this.router.navigateByUrl('/login');
				},
			});
	}
}
