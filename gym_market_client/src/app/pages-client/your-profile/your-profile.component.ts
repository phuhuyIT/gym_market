import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
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
import { DEFAULT_AVATAR_URL } from '../../utilities/defaults.const';

@Component({
    selector: 'app-your-profile',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink, CommonModule, GmButtonComponent],
    templateUrl: './your-profile.component.html',
    styleUrl: './your-profile.component.scss'
})
export class YourProfileComponent implements OnInit {
	readonly DEFAULT_AVATAR_URL = DEFAULT_AVATAR_URL;
	userStore = inject(UserStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
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
					this.cdr.markForCheck();
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
						this.cdr.markForCheck();
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
					this.cdr.markForCheck();
				},
				error: () => {
					this.router.navigateByUrl('/login');
				},
			});
	}
}
