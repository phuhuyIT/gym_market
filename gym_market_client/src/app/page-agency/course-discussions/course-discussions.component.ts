import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DiscussionAnswer, DiscussionQuestion } from '../../core/models/discussion.model';
import { DiscussionService } from '../../core/services/discussion.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-course-discussions',
	standalone: true,
	imports: [CommonModule, FormsModule, RouterLink],
	templateUrl: './course-discussions.component.html',
	styleUrl: './course-discussions.component.scss',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CourseDiscussionsComponent implements OnInit {
	courseId = '';
	questions: DiscussionQuestion[] = [];
	selectedQuestion: DiscussionQuestion | null = null;
	statusFilter = '';
	search = '';
	replyBody = '';
	isLoading = false;
	isThreadLoading = false;
	isReplying = false;

	private route = inject(ActivatedRoute);
	private discussionService = inject(DiscussionService);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit(): void {
		this.courseId = this.route.snapshot.params['courseId'];
		this.loadQuestions();
	}

	loadQuestions(): void {
		this.isLoading = true;
		this.discussionService
			.getCourseQuestions(this.courseId, { status: this.statusFilter, search: this.search })
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: questions => {
					this.questions = questions;
					this.isLoading = false;
					if (this.selectedQuestion) {
						const stillVisible = questions.some(q => q.questionId === this.selectedQuestion?.questionId);
						if (!stillVisible) this.selectedQuestion = null;
					}
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load discussions', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	selectQuestion(question: DiscussionQuestion): void {
		this.isThreadLoading = true;
		this.discussionService
			.getQuestion(question.questionId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: detail => {
					this.selectedQuestion = detail;
					this.isThreadLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.isThreadLoading = false;
					this.toastService.show('Failed to load thread', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	reply(): void {
		if (!this.selectedQuestion || this.isReplying) return;
		if (!this.replyBody.trim()) {
			this.toastService.show('Reply body is required', 'error');
			return;
		}

		this.isReplying = true;
		this.discussionService
			.createAnswer(this.selectedQuestion.questionId, { body: this.replyBody })
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.replyBody = '';
					this.isReplying = false;
					this.toastService.show('Reply posted');
					this.refreshSelected();
					this.loadQuestions();
				},
				error: err => {
					this.isReplying = false;
					this.toastService.show(err?.error?.message || err?.error?.Message || 'Failed to post reply', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	acceptAnswer(answer: DiscussionAnswer): void {
		if (!this.selectedQuestion) return;
		this.discussionService
			.acceptAnswer(this.selectedQuestion.questionId, answer.answerId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: question => {
					this.selectedQuestion = question;
					this.toastService.show('Answer accepted');
					this.loadQuestions();
					this.cdr.markForCheck();
				},
				error: () => this.toastService.show('Failed to accept answer', 'error'),
			});
	}

	setPinned(isPinned: boolean): void {
		if (!this.selectedQuestion) return;
		this.moderate({ isPinned });
	}

	setStatus(status: string): void {
		this.moderate({ status });
	}

	deleteQuestion(question: DiscussionQuestion): void {
		this.discussionService
			.deleteQuestion(question.questionId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					if (this.selectedQuestion?.questionId === question.questionId) this.selectedQuestion = null;
					this.toastService.show('Question deleted');
					this.loadQuestions();
				},
				error: () => this.toastService.show('Failed to delete question', 'error'),
			});
	}

	deleteAnswer(answer: DiscussionAnswer): void {
		this.discussionService
			.deleteAnswer(answer.answerId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.toastService.show('Answer deleted');
					this.refreshSelected();
					this.loadQuestions();
				},
				error: () => this.toastService.show('Failed to delete answer', 'error'),
			});
	}

	statusClass(status: string): string {
		return status.toLowerCase();
	}

	private moderate(model: { status?: string; isPinned?: boolean }): void {
		if (!this.selectedQuestion) return;
		this.discussionService
			.moderateQuestion(this.selectedQuestion.questionId, model)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: question => {
					this.selectedQuestion = question;
					this.toastService.show('Discussion updated');
					this.loadQuestions();
					this.cdr.markForCheck();
				},
				error: () => this.toastService.show('Failed to update discussion', 'error'),
			});
	}

	private refreshSelected(): void {
		if (!this.selectedQuestion) return;
		this.selectQuestion(this.selectedQuestion);
	}
}
