import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AssignmentService } from '../../core/services/assignment.service';
import { CourseAssignment } from '../../core/models/assignment.model';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-course-assignments',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, FormsModule, RouterLink, DatePipe, DecimalPipe],
	templateUrl: './course-assignments.component.html',
	styleUrl: './course-assignments.component.scss',
})
export class CourseAssignmentsComponent implements OnInit {
	courseId = '';
	assignments: CourseAssignment[] = [];
	selectedAssignment: CourseAssignment | null = null;
	textResponses: Record<string, string> = {};
	attachmentUrls: Record<string, string> = {};
	isLoading = false;
	isSubmitting = false;

	private route = inject(ActivatedRoute);
	private destroyRef = inject(DestroyRef);
	private assignmentService = inject(AssignmentService);
	private toastService = inject(ToastService);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit() {
		this.courseId = this.route.snapshot.params['courseId'];
		this.load();
	}

	load() {
		this.isLoading = true;
		this.assignmentService
			.getForStudent(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: assignments => {
					this.assignments = assignments;
					this.selectedAssignment = this.selectedAssignment
						? assignments.find(a => a.assignmentId === this.selectedAssignment?.assignmentId) ?? assignments[0] ?? null
						: assignments[0] ?? null;
					for (const assignment of assignments) {
						this.textResponses[assignment.assignmentId] = assignment.mySubmission?.textResponse ?? '';
						this.attachmentUrls[assignment.assignmentId] = assignment.mySubmission?.attachmentUrl ?? '';
					}
					this.isLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load assignments', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	selectAssignment(assignment: CourseAssignment) {
		this.selectedAssignment = assignment;
	}

	submitAssignment(assignment: CourseAssignment) {
		if (this.isSubmitting) return;
		const textResponse = this.textResponses[assignment.assignmentId]?.trim() || null;
		const attachmentUrl = this.attachmentUrls[assignment.assignmentId]?.trim() || null;
		if (!textResponse && !attachmentUrl) {
			this.toastService.show('Add a response or attachment link before submitting', 'error');
			return;
		}

		this.isSubmitting = true;
		this.assignmentService
			.submit(assignment.assignmentId, { textResponse, attachmentUrl })
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.isSubmitting = false;
					this.toastService.show('Assignment submitted');
					this.load();
				},
				error: err => {
					this.isSubmitting = false;
					const message = err?.error?.message || err?.error?.Message || 'Failed to submit assignment';
					this.toastService.show(message, 'error');
					this.cdr.markForCheck();
				},
			});
	}

	statusClass(assignment: CourseAssignment): string {
		const status = assignment.mySubmission?.status;
		if (status === 'Graded') return 'graded';
		if (status === 'Submitted') return 'submitted';
		return 'missing';
	}

	statusLabel(assignment: CourseAssignment): string {
		return assignment.mySubmission?.status || 'Missing';
	}
}
