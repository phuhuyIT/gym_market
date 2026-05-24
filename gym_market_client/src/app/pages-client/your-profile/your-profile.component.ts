import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { UserStore } from '../../stores/user.store';
import { StudentService } from '../student.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { StudentProfile } from '../../core/models/student.model';
import { CommonModule } from '@angular/common';
import { GmButtonComponent } from '../../shared';
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
		const userId = this.userStore.id();
		if (!userId) return;

		this.studentService
			.getStudentProfile(userId)
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
