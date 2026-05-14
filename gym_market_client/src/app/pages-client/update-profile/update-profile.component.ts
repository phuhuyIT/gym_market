import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserStore } from '../../stores/user.store';
import { LoaderModalStore } from '../../stores/loader.store';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { NoticeModalStore } from '../../stores/notice.store';
import { StudentService } from '../student.service';
import { Router } from '@angular/router';
import { UserService } from '../../user/user.service';
import { patchState } from '@ngrx/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UpdateStudentProfileDto } from '../../core/models/student.model';
import { UpdateUserDto } from '../../user/models/update-user.dto';

@Component({
	selector: 'app-update-profile',
	standalone: true,
	imports: [ReactiveFormsModule],
	templateUrl: './update-profile.component.html',
	styleUrl: './update-profile.component.scss',
})
export class UpdateProfileComponent implements OnInit {
	userStore = inject(UserStore);
	updateForm!: FormGroup;

	loader = inject(LoaderModalStore);
	errorModal = inject(ErrorModalStore);
	noticeModal = inject(NoticeModalStore);
	private destroyRef = inject(DestroyRef);

	constructor(
		private studentService: StudentService,
		private router: Router,
		private userService: UserService,
		private formBuilder: FormBuilder
	) {}

	ngOnInit() {
		this.updateForm = this.formBuilder.group({
			profilePicture: ['https://cdn-icons-png.flaticon.com/512/236/236832.png'],
			fullName: ['', [Validators.required]],
			address: ['', [Validators.required]],
			email: ['', [Validators.required, Validators.email]],
			phoneNumber: ['', [Validators.required]],
		});

		this.getUserInfo();
		this.getStudentInfo();
	}

	private getUserInfo() {
		const userId = this.userStore.id();
		if (userId) {
			this.userService
				.getUserInfo(userId)
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: (res) => {
						this.updateForm.patchValue({
							fullName: res.userInfo.fullName,
							address: res.userInfo.address,
							email: res.userInfo.email,
							phoneNumber: res.userInfo.phoneNumber,
						});
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
						this.updateForm.patchValue({
							profilePicture:
								res.profilePicture || 'https://cdn-icons-png.flaticon.com/512/236/236832.png',
						});
					},
					error: () => {
						this.router.navigateByUrl('/login');
					},
				});
		}
	}

	onUpdate() {
		if (this.updateForm.valid === false) {
			return;
		}

		const studentUpdate: UpdateStudentProfileDto = {
			fullName: this.updateForm.value.fullName,
			dateOfBirth: '', // Not in form, but required by DTO
			height: 0,
			weight: 0,
			avatar: this.updateForm.value.profilePicture,
		};

		const studentId = this.userStore.studentId();
		if (!studentId) return;

		patchState(this.loader, { isShow: true });
		this.studentService
			.updateStudentProfile(studentUpdate, studentId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.onUpdateUser();
				},
				error: err => {
					patchState(this.errorModal, {
						isShow: true,
						errors: err.error?.errors || ['Update failed'],
					});
					patchState(this.loader, { isShow: false });
				},
			});
	}

	onUpdateUser() {
		const user: UpdateUserDto = {
			address: this.updateForm.value.address,
			avatar: this.updateForm.value.profilePicture,
			fullName: this.updateForm.value.fullName,
			id: this.userStore.id(),
			phoneNumber: this.updateForm.value.phoneNumber,
			status: null,
		};

		this.userService
			.updateUser(user)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.loader, { isShow: false });
					patchState(this.noticeModal, { isShow: true, message: 'Update successfully' });
				},
				error: err => {
					patchState(this.errorModal, {
						isShow: true,
						errors: err.error?.errors || ['User update failed'],
					});
					patchState(this.loader, { isShow: false });
				},
			});
	}
}
