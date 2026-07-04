import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CourseStudyGroup, EligibleCourseLearner, UpsertCourseStudyGroup } from '../../core/models/course-study-group.model';
import { CourseStudyGroupService } from '../../core/services/course-study-group.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-course-cohorts',
	standalone: true,
	imports: [CommonModule, FormsModule, RouterLink],
	templateUrl: './course-cohorts.component.html',
	styleUrl: './course-cohorts.component.scss',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CourseCohortsComponent implements OnInit {
	courseId = '';
	groups: CourseStudyGroup[] = [];
	eligibleLearners: EligibleCourseLearner[] = [];
	selected: CourseStudyGroup | null = null;
	selectedLearnerIds = new Set<string>();
	isLoading = false;
	isSaving = false;

	form = this.emptyForm();

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
			.getForManagement(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: groups => {
					this.groups = groups;
					if (this.selected) {
						this.selected = groups.find(group => group.studyGroupId === this.selected?.studyGroupId) ?? null;
					} else {
						this.selected = groups[0] ?? null;
					}
					this.loadEligibleLearners();
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

	syncDefaultCohort(): void {
		this.isSaving = true;
		this.studyGroupService
			.syncDefaultCohort(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: cohort => {
					this.selected = cohort;
					this.toastService.show('Cohort synced from paid enrollments');
					this.isSaving = false;
					this.load();
				},
				error: () => {
					this.isSaving = false;
					this.toastService.show('Failed to sync cohort', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	selectGroup(group: CourseStudyGroup): void {
		this.selected = group;
		this.selectedLearnerIds.clear();
		this.form = {
			name: group.name,
			description: group.description || '',
			kind: group.kind || 'StudyGroup',
			isActive: group.isActive,
		};
		this.loadEligibleLearners();
	}

	newGroup(): void {
		this.selected = null;
		this.selectedLearnerIds.clear();
		this.form = this.emptyForm();
		this.loadEligibleLearners();
	}

	save(): void {
		if (this.isSaving) return;
		if (!this.form.name.trim()) {
			this.toastService.show('Group name is required', 'error');
			return;
		}

		const model: UpsertCourseStudyGroup = {
			name: this.form.name.trim(),
			description: this.form.description.trim() || null,
			kind: this.form.kind,
			isActive: this.form.isActive,
			memberUserIds: [...this.selectedLearnerIds],
		};

		this.isSaving = true;
		const wasEditing = !!this.selected;
		const request = this.selected
			? this.studyGroupService.update(this.selected.studyGroupId, model)
			: this.studyGroupService.create(this.courseId, model);

		request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: group => {
				this.selected = group;
				this.selectedLearnerIds.clear();
				this.toastService.show(wasEditing ? 'Study group saved' : 'Study group created');
				this.isSaving = false;
				this.load();
			},
			error: err => {
				this.isSaving = false;
				this.toastService.show(this.errorMessage(err, 'Failed to save study group'), 'error');
				this.cdr.markForCheck();
			},
		});
	}

	addSelectedLearners(): void {
		if (!this.selected || this.selected.isDefaultCohort || this.selectedLearnerIds.size === 0) return;
		this.studyGroupService
			.addMembers(this.selected.studyGroupId, [...this.selectedLearnerIds])
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.selectedLearnerIds.clear();
					this.toastService.show('Learners added');
					this.load();
				},
				error: err => this.toastService.show(this.errorMessage(err, 'Failed to add learners'), 'error'),
			});
	}

	removeMember(memberUserId: string): void {
		if (!this.selected || this.selected.isDefaultCohort) return;
		this.studyGroupService
			.removeMember(this.selected.studyGroupId, memberUserId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.toastService.show('Learner removed');
					this.load();
				},
				error: err => this.toastService.show(this.errorMessage(err, 'Failed to remove learner'), 'error'),
			});
	}

	setRole(memberUserId: string, role: 'Admin' | 'Member'): void {
		if (!this.selected) return;
		this.studyGroupService
			.updateMemberRole(this.selected.studyGroupId, memberUserId, role)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.toastService.show(role === 'Admin' ? 'Moderator added' : 'Moderator removed');
					this.load();
				},
				error: err => this.toastService.show(this.errorMessage(err, 'Failed to update role'), 'error'),
			});
	}

	archiveSelected(): void {
		if (!this.selected || this.selected.isDefaultCohort) return;
		this.studyGroupService
			.archive(this.selected.studyGroupId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.toastService.show('Study group archived');
					this.newGroup();
					this.load();
				},
				error: err => this.toastService.show(this.errorMessage(err, 'Failed to archive group'), 'error'),
			});
	}

	toggleLearner(userId: string, checked: boolean): void {
		if (checked) {
			this.selectedLearnerIds.add(userId);
		} else {
			this.selectedLearnerIds.delete(userId);
		}
	}

	availableLearners(): EligibleCourseLearner[] {
		return this.eligibleLearners.filter(learner => !learner.isInGroup);
	}

	loadEligibleLearners(): void {
		this.studyGroupService
			.getEligibleLearners(this.courseId, this.selected?.studyGroupId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: learners => {
					this.eligibleLearners = learners;
					this.cdr.markForCheck();
				},
				error: () => this.toastService.show('Failed to load eligible learners', 'error'),
			});
	}

	formatDate(value: string): string {
		return new Intl.DateTimeFormat(undefined, { month: 'short', day: 'numeric', hour: 'numeric', minute: '2-digit' }).format(new Date(value));
	}

	private emptyForm() {
		return {
			name: '',
			description: '',
			kind: 'StudyGroup',
			isActive: true,
		};
	}

	private errorMessage(err: unknown, fallback: string): string {
		const error = err as { error?: { errors?: string[] | string; message?: string; Message?: string } };
		const errors = error.error?.errors;
		return Array.isArray(errors) ? errors.join(', ') : errors || error.error?.message || error.error?.Message || fallback;
	}
}
