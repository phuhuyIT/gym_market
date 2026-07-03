import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DiscussionAnswer, DiscussionQuestion } from '../../core/models/discussion.model';
import { DiscussionService } from '../../core/services/discussion.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-client-course-discussions',
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
	isSavingQuestion = false;
	isReplying = false;
	newQuestion = {
		title: '',
		body: '',
	};

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
					if (!this.selectedQuestion && questions[0]) {
						this.selectQuestion(questions[0]);
					}
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load Q&A', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	askQuestion(): void {
		if (this.isSavingQuestion) return;
		if (!this.newQuestion.title.trim() || !this.newQuestion.body.trim()) {
			this.toastService.show('Question title and detail are required', 'error');
			return;
		}

		this.isSavingQuestion = true;
		this.discussionService
			.createQuestion(this.courseId, this.newQuestion)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: question => {
					this.isSavingQuestion = false;
					this.newQuestion = { title: '', body: '' };
					this.selectedQuestion = question;
					this.toastService.show('Question posted');
					this.loadQuestions();
					this.cdr.markForCheck();
				},
				error: err => {
					this.isSavingQuestion = false;
					this.toastService.show(err?.error?.message || err?.error?.Message || 'Failed to post question', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	selectQuestion(question: DiscussionQuestion): void {
		this.discussionService
			.getQuestion(question.questionId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: detail => {
					this.selectedQuestion = detail;
					this.cdr.markForCheck();
				},
				error: () => this.toastService.show('Failed to load thread', 'error'),
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
					this.isReplying = false;
					this.replyBody = '';
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

	deleteQuestion(question: DiscussionQuestion): void {
		this.discussionService
			.deleteQuestion(question.questionId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.toastService.show('Question deleted');
					this.selectedQuestion = null;
					this.loadQuestions();
				},
				error: () => this.toastService.show('Failed to delete question', 'error'),
			});
	}

	statusClass(status: string): string {
		return status.toLowerCase();
	}

	private refreshSelected(): void {
		if (!this.selectedQuestion) return;
		this.selectQuestion(this.selectedQuestion);
	}
}
