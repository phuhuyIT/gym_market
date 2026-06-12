import { Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { UserStore } from '../../stores/user.store';
import { Trainer, UpdateTrainerProfileDto } from '../../core/models/trainer.model';
import { TrainerService } from '../trainer.service';
import { Router } from '@angular/router';
import { UserService } from '../../user/user.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { UpdateUserDto } from '../../user/models/update-user.dto';
import { NoticeModalStore } from '../../stores/notice.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UserInfoResponse } from '../../core/models/auth.model';
import { GmButtonComponent } from '../../shared/components/gm-button/gm-button.component';
import { DEFAULT_AVATAR_URL } from '../../utilities/defaults.const';

@Component({
    selector: 'app-update-profile',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [ReactiveFormsModule, GmButtonComponent],
    templateUrl: './update-profile.component.html',
    styleUrl: './update-profile.component.scss'
})
export class UpdateProfileComponent implements OnInit {
	userStore = inject(UserStore);
	updateForm!: FormGroup;

	loader = inject(LoaderModalStore);
	errorModal = inject(ErrorModalStore);
	noticeModal = inject(NoticeModalStore);
	destroyRef = inject(DestroyRef);

	constructor(
		private trainerService: TrainerService,
		private router: Router,
		private userService: UserService,
		private formBuilder: FormBuilder
	) {}

	ngOnInit() {
		this.updateForm = this.formBuilder.group({
			profilePicture: [DEFAULT_AVATAR_URL],
			fullName: ['', [Validators.required]],
			address: ['', [Validators.required]],
			email: ['', [Validators.required, Validators.email]],
			phoneNumber: ['', [Validators.required]],
			bio: ['', [Validators.required]],
			experience: [0, [Validators.required, Validators.min(0)]],
			certification: ['', [Validators.required]],
		});

		this.getUserInfo();
		this.getTrainerInfo();
	}

	private getUserInfo() {
		const userId = this.userStore.id();
		if (userId !== null) {
			this.userService
				.getUserInfo()
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: (res: UserInfoResponse) => {
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

	private getTrainerInfo() {
		const trainerId = this.userStore.trainerId();
		if (trainerId) {
			this.trainerService
				.getTrainerInfo(trainerId)
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: (res: Trainer) => {
						this.updateForm.patchValue({
							profilePicture: res.profilePicture,
							bio: res.bio,
							experience: res.experience,
							certification: res.certification,
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

		const trainerUpdate: UpdateTrainerProfileDto = {
			trainerId: this.userStore.trainerId() ?? '',
			bio: this.updateForm.controls['bio'].value,
			certification: this.updateForm.controls['certification'].value,
			email: this.updateForm.controls['email'].value,
			experience: this.updateForm.controls['experience'].value,
			name: this.updateForm.controls['fullName'].value,
			profilePicture: this.updateForm.controls['profilePicture'].value,
			rating: 0,
			updatedAt: new Date(),
			userId: this.userStore.id() ?? '',
		};

		patchState(this.loader, { isShow: true });
		this.trainerService
			.updateTrainerProfile(trainerUpdate, this.userStore.trainerId() ?? '')
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.loader, { isShow: false });
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
		// The backend updates the authenticated user; no id is sent.
		const user: UpdateUserDto = {
			address: this.updateForm.controls['address'].value,
			avatar: this.updateForm.controls['profilePicture'].value,
			fullName: this.updateForm.controls['fullName'].value,
			phoneNumber: this.updateForm.controls['phoneNumber'].value,
			status: null,
		};

		patchState(this.loader, { isShow: true });
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
