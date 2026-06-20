import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { UserStore } from '../../stores/user.store';
import { StudentService } from '../student.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { StudentProfile } from '../../core/models/student.model';
import { CommonModule } from '@angular/common';
import { GmButtonComponent } from '../../shared';
import { patchState } from '@ngrx/signals';
import { DEFAULT_AVATAR_IMAGE_URL } from '../../utilities/defaults.const';
import { FallbackSrcDirective } from '../../shared/directives/fallback-src.directive';

@Component({
    selector: 'app-your-profile',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink, CommonModule, GmButtonComponent, FallbackSrcDirective],
    templateUrl: './your-profile.component.html',
    styleUrl: './your-profile.component.scss'
})
export class YourProfileComponent implements OnInit {
	readonly DEFAULT_AVATAR_IMAGE_URL = DEFAULT_AVATAR_IMAGE_URL;
	userStore = inject(UserStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	profile: StudentProfile | null = null;
	activeTab = 'OVERVIEW';

	constructor(
		private studentService: StudentService,
		private router: Router,
	) {}

	ngOnInit() {
		this.loadProfile();
	}

	private loadProfile() {
		// Stay logged-in gated, but the backend identifies the user from the JWT.
		if (!this.userStore.id()) return;

		this.studentService
			.getOwnStudentProfile()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.profile = res.studentProfile;
					if (res.studentProfile.studentId) {
						patchState(this.userStore, { studentId: res.studentProfile.studentId });
					}
					this.cdr.markForCheck();
				},
				error: () => {
					this.router.navigateByUrl('/login');
				},
			});
	}
}
