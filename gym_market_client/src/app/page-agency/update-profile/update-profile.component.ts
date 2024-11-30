import { Component, inject } from '@angular/core';
import { UserInfoDto } from '../../user/models/user-info.dto';
import { UserStore } from '../../stores/user.store';
import { TrainerInfoDto } from '../models/trainer-inf0.dto';
import { TrainerService } from '../trainer.service';
import { Router } from '@angular/router';
import { UserService } from '../../user/user.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UpdateTrainerProfileDto } from '../models/update-trainer.dto';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { UpdateUserDto } from '../../user/models/update-user.dto';
import { NoticeModalStore } from '../../stores/notice.store';

@Component({
	selector: 'app-update-profile',
	standalone: true,
	imports: [ReactiveFormsModule],
	templateUrl: './update-profile.component.html',
	styleUrl: './update-profile.component.scss',
})
export class UpdateProfileComponent {
	userStore = inject(UserStore);
	updateForm!: FormGroup;

	loader = inject(LoaderModalStore);
	errorModal = inject(ErrorModalStore);
	noticeModal = inject(NoticeModalStore);

	constructor(
		private trainerService: TrainerService,
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
			bio: ['', [Validators.required]],
			experience: [0, [Validators.required, Validators.min(0)]],
			certification: ['', [Validators.required]],
		});

		this.getUserInfo();
		this.getTrainerInfo();
	}

	private getUserInfo() {
		if (this.userStore.id() !== null) {
			this.userService.getUserInfo(this.userStore.id()).subscribe({
				next: (res: any) => {
					// this.userInfo = { ...res.userInfo };
					this.updateForm.controls['fullName'].setValue(res.userInfo.fullName);
					this.updateForm.controls['address'].setValue(res.userInfo.address);
					this.updateForm.controls['email'].setValue(res.userInfo.email);
					this.updateForm.controls['phoneNumber'].setValue(res.userInfo.phoneNumber);
				},
				error: error => {
					this.router.navigateByUrl('/login');
				},
			});
		}
	}

	private getTrainerInfo() {
		this.trainerService.getTrainerInfo(this.userStore.trainerId()).subscribe({
			next: (res: any) => {
				this.updateForm.controls['profilePicture'].setValue(res.profilePicture);
				this.updateForm.controls['bio'].setValue(res.bio);
				this.updateForm.controls['experience'].setValue(res.experience);
				this.updateForm.controls['certification'].setValue(res.certification);
			},
			error: error => {
				this.router.navigateByUrl('/login');
			},
		});
	}

	onUpdate() {
		if (this.updateForm.valid === false) {
			return;
		}

		// console.log(this.updateForm.value);
		const trainerUpdate: UpdateTrainerProfileDto = {
			trainerId: this.userStore.trainerId(),
			bio: this.updateForm.controls['bio'].value,
			certification: this.updateForm.controls['certification'].value,
			email: this.updateForm.controls['email'].value,
			experience: this.updateForm.controls['experience'].value,
			name: this.updateForm.controls['fullName'].value,
			profilePicture: this.updateForm.controls['profilePicture'].value,
			rating: 0,
			updatedAt: new Date(),
		};

		patchState(this.loader, { isShow: true });
		this.trainerService
			.updateTrainerProfile(trainerUpdate, this.userStore.trainerId())
			.subscribe({
				next: (res: any) => {
					console.log(res);
					patchState(this.loader, { isShow: false });
					this.onUpdateUser();
				},
				error: (err: any) => {
					console.log(err);
					patchState(this.errorModal, { isShow: true, errors: err.error.errors });
					patchState(this.loader, { isShow: false });
				},
			});
	}

	onUpdateUser() {
		const user: UpdateUserDto = {
			address: this.updateForm.controls['address'].value,
			avatar: this.updateForm.controls['profilePicture'].value,
			fullName: this.updateForm.controls['fullName'].value,
			id: this.userStore.id(),
			phoneNumber: this.updateForm.controls['phoneNumber'].value,
			status: null,
		};

		patchState(this.loader, { isShow: true });
		this.userService.updateUser(user).subscribe({
			next: (res: any) => {
				console.log(res);
				patchState(this.loader, { isShow: false });
				patchState(this.noticeModal, { isShow: true, message: 'Update successfully' });
			},
			error: err => {
				console.log(err);
				patchState(this.errorModal, { isShow: true, errors: err.error.errors });
				patchState(this.loader, { isShow: false });
			},
		});
	}
}
