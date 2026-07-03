import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AssignmentService } from '../../core/services/assignment.service';
import { GradebookService } from '../../core/services/gradebook.service';
import {
	AssignmentSubmission,
	CourseAssignment,
	GradeAssignmentRubricScore,
	UpsertAssignmentRubricCriterion,
	UpsertCourseAssignment,
} from '../../core/models/assignment.model';
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
	rubricScoreValues: Record<string, number> = {};
	rubricFeedbackValues: Record<string, string> = {};
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
			rubricCriteria: (assignment.rubricCriteria ?? []).map(criterion => ({
				criterionId: criterion.criterionId,
				title: criterion.title,
				description: criterion.description ?? '',
				pointsPossible: criterion.pointsPossible,
				order: criterion.order,
			})),
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
		const rubricCriteria = this.draft.rubricCriteria.map((criterion, index) => ({
			...criterion,
			pointsPossible: Number(criterion.pointsPossible) || 0,
			order: index + 1,
		}));
		const payload = {
			...this.draft,
			dueAt: this.draft.dueAt || null,
			pointsPossible: rubricCriteria.length > 0 ? this.rubricTotal(rubricCriteria) : Number(this.draft.pointsPossible) || 100,
			rubricCriteria,
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
					this.rubricScoreValues = {};
					this.rubricFeedbackValues = {};
					for (const submission of submissions) {
						for (const criterion of assignment.rubricCriteria ?? []) {
							const existing = submission.rubricScores?.find(score => score.criterionId === criterion.criterionId);
							const key = this.rubricKey(submission.submissionId, criterion.criterionId);
							this.rubricScoreValues[key] = existing?.score ?? 0;
							this.rubricFeedbackValues[key] = existing?.feedback ?? '';
						}
					}
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
		const rubricScores = this.buildRubricScores(submission);
		if (rubricScores === null) return;
		const score = rubricScores.length > 0
			? rubricScores.reduce((sum, item) => sum + item.score, 0)
			: Number(this.gradeScores[submission.submissionId]);
		if (Number.isNaN(score) || score < 0 || score > this.selectedAssignment.pointsPossible) {
			this.toastService.show(`Score must be between 0 and ${this.selectedAssignment.pointsPossible}`, 'error');
			return;
		}
		this.assignmentService
			.grade(submission.submissionId, rubricScores.length > 0 ? null : score, this.gradeFeedbacks[submission.submissionId], rubricScores)
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

	addRubricCriterion() {
		const nextOrder = this.draft.rubricCriteria.length + 1;
		this.draft.rubricCriteria = [
			...this.draft.rubricCriteria,
			{
				title: `Criterion ${nextOrder}`,
				description: '',
				pointsPossible: 10,
				order: nextOrder,
			},
		];
		this.syncPointsWithRubric();
	}

	removeRubricCriterion(index: number) {
		this.draft.rubricCriteria = this.draft.rubricCriteria
			.filter((_, i) => i !== index)
			.map((criterion, order) => ({ ...criterion, order: order + 1 }));
		this.syncPointsWithRubric();
	}

	syncPointsWithRubric() {
		if (this.draft.rubricCriteria.length === 0) return;
		this.draft.pointsPossible = this.rubricTotal(this.draft.rubricCriteria);
	}

	rubricTotal(criteria: UpsertAssignmentRubricCriterion[] = this.draft.rubricCriteria): number {
		return criteria.reduce((sum, criterion) => sum + (Number(criterion.pointsPossible) || 0), 0);
	}

	rubricKey(submissionId: string, criterionId: string): string {
		return `${submissionId}:${criterionId}`;
	}

	private buildRubricScores(submission: AssignmentSubmission): GradeAssignmentRubricScore[] | null {
		const criteria = this.selectedAssignment?.rubricCriteria ?? [];
		if (criteria.length === 0) return [];

		const scores: GradeAssignmentRubricScore[] = [];
		for (const criterion of criteria) {
			const key = this.rubricKey(submission.submissionId, criterion.criterionId);
			const score = Number(this.rubricScoreValues[key]);
			if (Number.isNaN(score) || score < 0 || score > criterion.pointsPossible) {
				this.toastService.show(`"${criterion.title}" must be between 0 and ${criterion.pointsPossible}`, 'error');
				return null;
			}

			scores.push({
				criterionId: criterion.criterionId,
				score,
				feedback: this.rubricFeedbackValues[key] || null,
			});
		}

		return scores;
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
			rubricCriteria: [],
		};
	}

	private toDateTimeLocal(value?: string | null): string | null {
		if (!value) return null;
		const date = new Date(value);
		const pad = (n: number) => `${n}`.padStart(2, '0');
		return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
	}
}
