import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AssignmentService } from '../../core/services/assignment.service';
import { GradebookService } from '../../core/services/gradebook.service';
import { AssignmentSubmission, CourseAssignment, UpsertCourseAssignment } from '../../core/models/assignment.model';
import { GradeCategory } from '../../core/models/gradebook.model';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-assignments',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, FormsModule, RouterLink],
	templateUrl: './assignments.component.html',
	styleUrl: './assignments.component.scss',
})
export class AssignmentsComponent implements OnInit {
	courseId = '';
	assignments: CourseAssignment[] = [];
	categories: GradeCategory[] = [];
	selectedAssignment: CourseAssignment | null = null;
	submissions: AssignmentSubmission[] = [];
	gradeScores: Record<string, number> = {};
	gradeFeedbacks: Record<string, string> = {};
	isLoading = false;
	isSaving = false;
	isSubmissionsLoading = false;

	draft: UpsertCourseAssignment = this.emptyDraft();

	private route = inject(ActivatedRoute);
	private destroyRef = inject(DestroyRef);
	private assignmentService = inject(AssignmentService);
	private gradebookService = inject(GradebookService);
	private toastService = inject(ToastService);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit() {
		this.courseId = this.route.snapshot.params['courseId'];
		this.loadCategories();
		this.loadAssignments();
	}

	loadCategories() {
		this.gradebookService
			.getPolicy(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: policy => {
					this.categories = policy.categories;
					if (!this.draft.gradeCategoryId && this.categories[0]) {
						this.draft.gradeCategoryId = this.categories[0].categoryId;
					}
					this.cdr.markForCheck();
				},
			});
	}

	loadAssignments() {
		this.isLoading = true;
		this.assignmentService
			.getForManagement(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: assignments => {
					this.assignments = assignments;
					this.isLoading = false;
					if (this.selectedAssignment) {
						this.selectedAssignment = assignments.find(a => a.assignmentId === this.selectedAssignment?.assignmentId) ?? null;
					}
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load assignments', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	editAssignment(assignment: CourseAssignment) {
		this.selectedAssignment = assignment;
		this.draft = {
			title: assignment.title,
			instructions: assignment.instructions ?? '',
			gradeCategoryId: assignment.gradeCategoryId ?? this.categories[0]?.categoryId ?? null,
			pointsPossible: assignment.pointsPossible,
			dueAt: this.toDateTimeLocal(assignment.dueAt),
			submissionType: assignment.submissionType,
			status: assignment.status,
		};
		this.loadSubmissions(assignment);
	}

	newAssignment() {
		this.selectedAssignment = null;
		this.submissions = [];
		this.draft = this.emptyDraft();
	}

	saveAssignment() {
		if (this.isSaving) return;
		if (!this.draft.title.trim()) {
			this.toastService.show('Assignment title is required', 'error');
			return;
		}
		this.isSaving = true;
		const payload = {
			...this.draft,
			dueAt: this.draft.dueAt || null,
			pointsPossible: Number(this.draft.pointsPossible) || 100,
		};
		const request = this.selectedAssignment
			? this.assignmentService.update(this.selectedAssignment.assignmentId, payload)
			: this.assignmentService.create(this.courseId, payload);

		request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: assignment => {
				this.isSaving = false;
				this.toastService.show(this.selectedAssignment ? 'Assignment saved' : 'Assignment created');
				this.selectedAssignment = assignment;
				this.loadAssignments();
				this.loadSubmissions(assignment);
			},
			error: err => {
				this.isSaving = false;
				const message = err?.error?.message || err?.error?.Message || 'Failed to save assignment';
				this.toastService.show(message, 'error');
				this.cdr.markForCheck();
			},
		});
	}

	deleteAssignment(assignment: CourseAssignment) {
		this.assignmentService
			.remove(assignment.assignmentId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.toastService.show('Assignment deleted');
					if (this.selectedAssignment?.assignmentId === assignment.assignmentId) {
						this.newAssignment();
					}
					this.loadAssignments();
				},
				error: () => this.toastService.show('Failed to delete assignment', 'error'),
			});
	}

	loadSubmissions(assignment: CourseAssignment) {
		this.isSubmissionsLoading = true;
		this.assignmentService
			.getSubmissions(assignment.assignmentId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: submissions => {
					this.submissions = submissions;
					this.gradeScores = Object.fromEntries(submissions.map(s => [s.submissionId, s.score ?? 0]));
					this.gradeFeedbacks = Object.fromEntries(submissions.map(s => [s.submissionId, s.feedback ?? '']));
					this.isSubmissionsLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.isSubmissionsLoading = false;
					this.toastService.show('Failed to load submissions', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	gradeSubmission(submission: AssignmentSubmission) {
		if (!this.selectedAssignment) return;
		const score = Number(this.gradeScores[submission.submissionId]);
		if (Number.isNaN(score) || score < 0 || score > this.selectedAssignment.pointsPossible) {
			this.toastService.show(`Score must be between 0 and ${this.selectedAssignment.pointsPossible}`, 'error');
			return;
		}
		this.assignmentService
			.grade(submission.submissionId, score, this.gradeFeedbacks[submission.submissionId])
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: updated => {
					this.submissions = this.submissions.map(item => item.submissionId === updated.submissionId ? updated : item);
					this.toastService.show('Submission graded');
					this.loadAssignments();
					this.cdr.markForCheck();
				},
				error: () => this.toastService.show('Failed to grade submission', 'error'),
			});
	}

	statusClass(status: string): string {
		return status === 'Published' ? 'published' : status === 'Closed' ? 'closed' : 'draft';
	}

	private emptyDraft(): UpsertCourseAssignment {
		return {
			title: '',
			instructions: '',
			gradeCategoryId: this.categories[0]?.categoryId ?? null,
			pointsPossible: 100,
			dueAt: null,
			submissionType: 'Text',
			status: 'Draft',
		};
	}

	private toDateTimeLocal(value?: string | null): string | null {
		if (!value) return null;
		const date = new Date(value);
		const pad = (n: number) => `${n}`.padStart(2, '0');
		return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
	}
}
