import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { UserService } from '../../user/user.service';
import { UserStore } from '../../stores/user.store';
import { StudentService } from '../student.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Student } from '../../core/models/student.model';
import { UserInfo, UserInfoResponse } from '../../core/models/auth.model';
import { NgIf } from '@angular/common';

@Component({
	selector: 'app-your-profile',
	standalone: true,
	imports: [RouterLink, NgIf],
	templateUrl: './your-profile.component.html',
	styleUrl: './your-profile.component.scss',
})
export class YourProfileComponent implements OnInit {
	userStore = inject(UserStore);
	private destroyRef = inject(DestroyRef);
	studentInfo: Student | null = null;
	userInfo: UserInfo | null = null;

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
		if (userId !== null) {
			this.userService
				.getUserInfo(userId)
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: (res: UserInfoResponse) => {
						this.userInfo = res.userInfo;
						if (this.userInfo && !this.userInfo.avatar) {
							this.userInfo.avatar =
								'https://cdn-icons-png.flaticon.com/512/236/236832.png';
						}
					},
					error: () => {
						this.router.navigateByUrl('/login');
					},
				});
		}
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
						if (this.studentInfo && !this.studentInfo.profilePicture) {
							this.studentInfo.profilePicture =
								'https://cdn-icons-png.flaticon.com/512/236/236832.png';
						}
					},
					error: () => {
						this.router.navigateByUrl('/login');
					},
				});
		}
	}
}
