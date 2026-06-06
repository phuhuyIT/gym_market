import { Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserStore } from '../../stores/user.store';
import { ALLOWED_IMAGE_TYPES, MAX_AVATAR_BYTES } from '../../utilities/upload.const';
import { LoaderModalStore } from '../../stores/loader.store';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { NoticeModalStore } from '../../stores/notice.store';
import { StudentService } from '../student.service';
import { Router, RouterLink } from '@angular/router';
import { UserService } from '../../user/user.service';
import { patchState } from '@ngrx/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UpdateStudentProfileDto } from '../../core/models/student.model';
import { UpdateUserDto } from '../../user/models/update-user.dto';
import { HttpErrorBody } from '../../core/models/auth.model';
import { GmButtonComponent } from '../../shared/components/gm-button/gm-button.component';
import { DEFAULT_AVATAR_URL } from '../../utilities/defaults.const';
import { AccountService } from '../../guest/account.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
    selector: 'app-update-profile',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [ReactiveFormsModule, GmButtonComponent, RouterLink],
    templateUrl: './update-profile.component.html',
    styleUrl: './update-profile.component.scss'
})
export class UpdateProfileComponent implements OnInit {
	readonly DEFAULT_AVATAR_URL = DEFAULT_AVATAR_URL;
	userStore = inject(UserStore);
	updateForm!: FormGroup;

	loader = inject(LoaderModalStore);
	errorModal = inject(ErrorModalStore);
	noticeModal = inject(NoticeModalStore);
	private destroyRef = inject(DestroyRef);

	showPasswordModal = false;
	avatarPreviewUrl: string | null = null;
	private avatarFile: File | null = null;

	private accountService = inject(AccountService);
	private toastService = inject(ToastService);

	constructor(
		private studentService: StudentService,
		private router: Router,
		private userService: UserService,
		private formBuilder: FormBuilder
	) {}

	onAvatarFileSelected(event: Event) {
		const input = event.target as HTMLInputElement;
		if (!input.files?.length) return;
		const file = input.files[0];

		if (!ALLOWED_IMAGE_TYPES.includes(file.type)) {
			this.toastService.show('Only JPEG, PNG, WebP, and GIF images are allowed', 'error');
			return;
		}
		if (file.size > MAX_AVATAR_BYTES) {
			this.toastService.show('File must be under 5 MB', 'error');
			return;
		}

		this.avatarFile = file;
		const reader = new FileReader();
		reader.onload = () => (this.avatarPreviewUrl = reader.result as string);
		reader.readAsDataURL(file);
	}

	private uploadAvatarIfNeeded(): Promise<void> {
		if (!this.avatarFile) return Promise.resolve();
		return new Promise((resolve, reject) => {
			this.accountService
				.uploadAvatar(this.avatarFile!)
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: res => {
						if (res.success) {
							this.updateForm.patchValue({ profilePicture: res.avatarUrl });
							patchState(this.userStore, { avatar: res.avatarUrl });
							this.avatarFile = null;
						}
						resolve();
					},
					error: () => reject(),
				});
		});
	}

	ngOnInit() {
		this.updateForm = this.formBuilder.group({
			profilePicture: [DEFAULT_AVATAR_URL],
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
								res.profilePicture || DEFAULT_AVATAR_URL,
						});
					},
					error: () => {
						this.router.navigateByUrl('/login');
					},
				});
		} else {
			const userId = this.userStore.id();
			if (userId) {
				this.studentService
					.getStudentInfoByUserId(userId)
					.pipe(takeUntilDestroyed(this.destroyRef))
					.subscribe({
						next: res => {
							if (res.studentId) {
								patchState(this.userStore, { studentId: res.studentId });
							}
							this.updateForm.patchValue({
								profilePicture:
									res.profilePicture || DEFAULT_AVATAR_URL,
							});
						},
						error: () => {
							this.router.navigateByUrl('/login');
						},
					});
			}
		}
	}

	private getErrors(err: HttpErrorBody): string[] {
		if (err.error?.errors) {
			if (Array.isArray(err.error.errors)) {
				return err.error.errors;
			} else if (typeof err.error.errors === 'object') {
				return Object.values(err.error.errors).flat() as string[];
			} else if (typeof err.error.errors === 'string') {
				return [err.error.errors];
			}
		}
		if (err.error?.message) {
			return [err.error.message];
		}
		return [err.message || 'Update failed'];
	}

	onUpdate() {
		if (this.updateForm.valid === false) {
			return;
		}
		this.showPasswordModal = true;
	}

	closePasswordModal() {
		this.showPasswordModal = false;
	}

	async confirmUpdate(password: string) {
		if (!password || password.trim().length === 0) {
			patchState(this.errorModal, {
				isShow: true,
				errors: ['Password is required to confirm changes.'],
			});
			return;
		}

		const studentId = this.userStore.studentId();
		if (!studentId) return;

		this.showPasswordModal = false;
		patchState(this.loader, { isShow: true });

		try {
			await this.uploadAvatarIfNeeded();
		} catch {
			patchState(this.loader, { isShow: false });
			return;
		}

		const formValues = this.updateForm.getRawValue();

		const studentUpdate: UpdateStudentProfileDto = {
			studentId: studentId,
			name: formValues.fullName,
			email: formValues.email,
			profilePicture: formValues.profilePicture,
			password: password,
		};

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
						errors: this.getErrors(err),
					});
					patchState(this.loader, { isShow: false });
				},
			});
	}

	onUpdateUser() {
		const formValues = this.updateForm.getRawValue();
		const user: UpdateUserDto = {
			address: formValues.address,
			avatar: formValues.profilePicture,
			fullName: formValues.fullName,
			id: this.userStore.id(),
			phoneNumber: formValues.phoneNumber,
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
						errors: this.getErrors(err),
					});
					patchState(this.loader, { isShow: false });
				},
			});
	}
}
