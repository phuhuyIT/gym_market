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
	standalone: true,
	imports: [RouterLink, CommonModule, GmCardComponent, GmButtonComponent],
	templateUrl: './your-profile.component.html',
	styleUrl: './your-profile.component.scss',
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
		console.log('YourProfileComponent: OnInit called. userStore id:', this.userStore.id(), 'studentId:', this.userStore.studentId());
		this.getUserInfo();
		this.getStudentInfo();
	}

	private getUserInfo() {
		const userId = this.userStore.id();
		if (userId !== null) {
			console.log('YourProfileComponent: Fetching user info for userId:', userId);
			this.userService
				.getUserInfo(userId)
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: (res: UserInfoResponse) => {
						console.log('YourProfileComponent: Fetching user info successful. Response:', res);
						this.userInfo = res.userInfo;
					},
					error: (err) => {
						console.error('YourProfileComponent: Fetching user info failed:', err);
						this.router.navigateByUrl('/login');
					},
				});
		} else {
			console.warn('YourProfileComponent: Fetching user info skipped. userId is null.');
		}
	}

	private getStudentInfo() {
		const studentId = this.userStore.studentId();
		if (studentId) {
			console.log('YourProfileComponent: Fetching student info for studentId:', studentId);
			this.studentService
				.getStudentInfo(studentId)
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: res => {
						console.log('YourProfileComponent: Fetching student info successful. Response:', res);
						this.studentInfo = res;
					},
					error: (err) => {
						console.error('YourProfileComponent: Fetching student info failed:', err);
						this.router.navigateByUrl('/login');
					},
				});
		} else {
			console.warn('YourProfileComponent: Fetching student info skipped. studentId is falsy. Trying fallback by userId.');
			const userId = this.userStore.id();
			if (userId) {
				this.studentService
					.getStudentInfoByUserId(userId)
					.pipe(takeUntilDestroyed(this.destroyRef))
					.subscribe({
						next: res => {
							console.log('YourProfileComponent: Fallback fetching student info successful. Response:', res);
							this.studentInfo = res;
							if (res.studentId) {
								patchState(this.userStore, { studentId: res.studentId });
							}
						},
						error: (err) => {
							console.error('YourProfileComponent: Fallback fetching student info failed:', err);
							this.router.navigateByUrl('/login');
						},
					});
			} else {
				console.warn('YourProfileComponent: Fetching student info skipped. userId is also falsy.');
			}
		}
	}
}
