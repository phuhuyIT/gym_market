import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CourseStudyGroup } from '../../core/models/course-study-group.model';
import { CourseStudyGroupService } from '../../core/services/course-study-group.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-client-course-cohorts',
	standalone: true,
	imports: [CommonModule, RouterLink],
	templateUrl: './course-cohorts.component.html',
	styleUrl: './course-cohorts.component.scss',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CourseCohortsComponent implements OnInit {
	courseId = '';
	groups: CourseStudyGroup[] = [];
	isLoading = false;

	private route = inject(ActivatedRoute);
	private studyGroupService = inject(CourseStudyGroupService);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit(): void {
		this.courseId = this.route.snapshot.params['courseId'];
		this.load();
	}

	load(): void {
		this.isLoading = true;
		this.studyGroupService
			.getForStudent(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: groups => {
					this.groups = groups;
					this.isLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load course groups', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	moderators(group: CourseStudyGroup) {
		return group.members.filter(member => member.role === 'Owner' || member.role === 'Admin');
	}

	formatDate(value: string): string {
		return new Intl.DateTimeFormat(undefined, { month: 'short', day: 'numeric' }).format(new Date(value));
	}
}
