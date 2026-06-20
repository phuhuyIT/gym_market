import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TrainerService } from '../trainer.service';
import { UserStore } from '../../stores/user.store';
import { UserService } from '../../user/user.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Trainer } from '../../core/models/trainer.model';
import { UserInfo, UserInfoResponse } from '../../core/models/auth.model';
import { CommonModule } from '@angular/common';
import { GmButtonComponent } from '../../shared';
import { DEFAULT_AVATAR_IMAGE_URL, DEFAULT_AVATAR_URL } from '../../utilities/defaults.const';
import { FallbackSrcDirective } from '../../shared/directives/fallback-src.directive';

@Component({
    selector: 'app-your-profile',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink, CommonModule, GmButtonComponent, FallbackSrcDirective],
    templateUrl: './your-profile.component.html',
    styleUrl: './your-profile.component.scss'
})
export class YourProfileComponent implements OnInit {
	readonly DEFAULT_AVATAR_URL = DEFAULT_AVATAR_URL;
	readonly DEFAULT_AVATAR_IMAGE_URL = DEFAULT_AVATAR_IMAGE_URL;
	userInfo: UserInfo | null = null;
	userStore = inject(UserStore);
	trainerInfo: Trainer | null = null;
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	get certificationsList(): string[] {
		if (!this.trainerInfo || !this.trainerInfo.certification) return [];
		return this.trainerInfo.certification
			.split(/[\n,;]/)
			.map(c => c.trim())
			.filter(c => c.length > 0);
	}

	constructor(
		private trainerService: TrainerService,
		private router: Router,
		private userService: UserService
	) {}

	ngOnInit() {
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
						this.userInfo = res.userInfo;
						if (this.userInfo && !this.userInfo.avatar) {
							this.userInfo.avatar =
								DEFAULT_AVATAR_URL;
						}
						this.cdr.markForCheck();
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
						this.trainerInfo = res;
						if (this.trainerInfo && !this.trainerInfo.profilePicture) {
							this.trainerInfo.profilePicture =
								DEFAULT_AVATAR_URL;
						}
						this.cdr.markForCheck();
					},
					error: () => {
						this.router.navigateByUrl('/login');
					},
				});
		}
	}
}
