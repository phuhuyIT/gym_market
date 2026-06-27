import {
	ChangeDetectionStrategy,
	ChangeDetectorRef,
	Component,
	DestroyRef,
	inject,
	OnInit,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { patchState } from '@ngrx/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';
import { GmButtonComponent, GmCardComponent } from '../../shared';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { CourseMaterialService } from '../../page-agency/course-material.service';
import { Course } from '../../core/models/course.model';
import { Lecture, LectureMaterial } from '../../core/models/lecture.model';
import { CourseQuiz, QuizAttemptSummary } from '../../core/models/quiz.model';
import { CourseCertificate, CourseCompletionStatus } from '../../core/models/certificate.model';
import { STORAGE_KEYS } from '../../utilities/storage-keys.const';

@Component({
	selector: 'app-course-learn',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, GmButtonComponent, GmCardComponent],
	templateUrl: './course-learn.component.html',
	styleUrl: './course-learn.component.scss',
})
export class CourseLearnComponent implements OnInit {
	courseId = '';
	course: Course | null = null;

	lectures: Lecture[] = [];
	selectedLecture: Lecture | null = null;
	materials: LectureMaterial[] = [];
	quiz: CourseQuiz | null = null;
	quizAnswers: Record<string, string> = {};
	latestQuizAttempt: QuizAttemptSummary | null = null;
	isQuizSubmitting = false;
	completionStatus: CourseCompletionStatus | null = null;
	issuedCertificate: CourseCertificate | null = null;
	isCertificateIssuing = false;

		// The backend payment-gates lecture access and returns 403 for students
		// who have not paid; we surface that as a friendly locked state.
		accessDenied = false;

		private completedLectureIds = new Set<string>();

	loader = inject(LoaderModalStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private route = inject(ActivatedRoute);
	private router = inject(Router);
	private courseService = inject(CourseAgencyService);
	private courseMaterialService = inject(CourseMaterialService);

	ngOnInit() {
		this.route.params.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
				next: params => {
					this.courseId = params['courseId'];
					this.completedLectureIds = this.readCompleted(this.courseId);
					this.loadCourse();
					this.loadLectures();
					this.loadQuiz();
					this.loadCompletionStatus();
				},
			});
	}

	private loadCourse() {
		this.courseService
			.getCourse(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.course = res;
					this.cdr.markForCheck();
				},
				error: () => {},
			});
	}

	private loadLectures() {
		patchState(this.loader, { isShow: true });
		this.courseMaterialService
			.getLecturesByCourse(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
					next: res => {
						this.lectures = res.sort((a, b) => (a.order ?? 0) - (b.order ?? 0));
						if (this.lectures.length > 0) {
							this.selectLecture(this.lectures[0]);
						}
						this.loadProgress();
						patchState(this.loader, { isShow: false });
						this.cdr.markForCheck();
					},
				error: err => {
					patchState(this.loader, { isShow: false });
					if (err.status === 403) {
						this.accessDenied = true;
					} else {
						this.toastService.show('Failed to load course content', 'error');
					}
					this.cdr.markForCheck();
				},
			});
	}

	private loadProgress() {
		this.courseMaterialService
			.getCourseProgress(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.completedLectureIds = new Set(res.completedLectureIds ?? []);
					this.writeCompleted(this.courseId, this.completedLectureIds);
					this.loadCompletionStatus();
					this.cdr.markForCheck();
				},
				error: () => {
					// Keep the local cache as a non-authoritative offline fallback.
					this.cdr.markForCheck();
				},
			});
	}

	private loadQuiz() {
		this.quiz = null;
		this.latestQuizAttempt = null;
		this.quizAnswers = {};
		this.courseMaterialService
			.getQuiz(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: quiz => {
					this.quiz = quiz;
					this.latestQuizAttempt = quiz.latestAttempt ?? null;
					this.cdr.markForCheck();
				},
				error: err => {
					if (err.status !== 404 && err.status !== 403) {
						this.toastService.show('Failed to load quiz', 'error');
					}
					this.cdr.markForCheck();
				},
			});
	}

	private loadCompletionStatus() {
		this.courseMaterialService
			.getCompletionStatus(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: status => {
					this.completionStatus = status;
					this.issuedCertificate = status.certificate ?? null;
					this.cdr.markForCheck();
				},
				error: err => {
					if (err.status !== 403 && err.status !== 404) {
						this.toastService.show('Failed to load completion status', 'error');
					}
					this.cdr.markForCheck();
				},
			});
	}

	selectLecture(lecture: Lecture) {
		this.selectedLecture = lecture;
		this.materials = [];
		patchState(this.loader, { isShow: true });
		this.courseMaterialService
			.getMaterialsByLecture(lecture.lectureId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.materials = res;
					patchState(this.loader, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loader, { isShow: false });
					this.toastService.show('Failed to load materials', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	// Normalised, case-insensitive type used by the template's @switch.
	materialType(material: LectureMaterial): string {
		return (material.materialType ?? '').trim().toUpperCase();
	}

	// ---------- Local completion tracking ----------
	isCompleted(lectureId: string): boolean {
		return this.completedLectureIds.has(lectureId);
	}

	toggleCompleted(lecture: Lecture) {
		const wasCompleted = this.completedLectureIds.has(lecture.lectureId);
		if (this.completedLectureIds.has(lecture.lectureId)) {
			this.completedLectureIds.delete(lecture.lectureId);
		} else {
			this.completedLectureIds.add(lecture.lectureId);
		}
		this.writeCompleted(this.courseId, this.completedLectureIds);
		this.cdr.markForCheck();

		this.courseMaterialService
			.updateLectureProgress(lecture.lectureId, !wasCompleted)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {},
				error: () => {
					if (wasCompleted) {
						this.completedLectureIds.add(lecture.lectureId);
					} else {
						this.completedLectureIds.delete(lecture.lectureId);
					}
					this.writeCompleted(this.courseId, this.completedLectureIds);
					this.toastService.show('Failed to update progress', 'error');
					this.cdr.markForCheck();
				},
				complete: () => this.loadCompletionStatus(),
			});
	}

	get completedCount(): number {
		return this.lectures.filter(l => this.completedLectureIds.has(l.lectureId)).length;
	}

	get progressPercent(): number {
		if (this.lectures.length === 0) return 0;
		return Math.round((this.completedCount / this.lectures.length) * 100);
	}

	goToDetails() {
		this.router.navigate(['/client/course-details', this.courseId]);
	}

	selectQuizAnswer(questionId: string, optionId: string) {
		this.quizAnswers[questionId] = optionId;
	}

	submitQuiz() {
		if (!this.quiz || this.isQuizSubmitting) return;
		const unanswered = this.quiz.questions.some(question => !this.quizAnswers[question.questionId]);
		if (unanswered) {
			this.toastService.show('Answer every question before submitting', 'error');
			return;
		}

		this.isQuizSubmitting = true;
		this.courseMaterialService
			.submitQuiz(this.courseId, {
				answers: this.quiz.questions.map(question => ({
					questionId: question.questionId,
					selectedOptionId: this.quizAnswers[question.questionId],
				})),
			})
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: attempt => {
					this.latestQuizAttempt = attempt;
					if (this.quiz) {
						this.quiz.latestAttempt = attempt;
						if (!this.quiz.bestAttempt || attempt.scorePercent > this.quiz.bestAttempt.scorePercent) {
							this.quiz.bestAttempt = attempt;
						}
					}
					this.quizAnswers = {};
					this.isQuizSubmitting = false;
					this.toastService.show(attempt.passed ? 'Quiz passed' : 'Quiz submitted');
					this.loadCompletionStatus();
					this.cdr.markForCheck();
				},
				error: () => {
					this.isQuizSubmitting = false;
					this.toastService.show('Failed to submit quiz', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	issueCertificate() {
		if (!this.completionStatus?.isCompleted || this.isCertificateIssuing) return;
		this.isCertificateIssuing = true;
		this.courseMaterialService
			.issueCertificate(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: certificate => {
					this.issuedCertificate = certificate;
					if (this.completionStatus) {
						this.completionStatus.certificate = certificate;
					}
					this.isCertificateIssuing = false;
					this.toastService.show('Certificate issued');
					this.cdr.markForCheck();
				},
				error: err => {
					this.isCertificateIssuing = false;
					if (err.status === 409 && err.error) {
						this.completionStatus = err.error;
					}
					this.toastService.show('Complete all requirements before issuing a certificate', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	private storageKey(): string {
		return STORAGE_KEYS.completedLectures;
	}

	private readCompleted(courseId: string): Set<string> {
		try {
			const raw = localStorage.getItem(this.storageKey());
			const map: Record<string, string[]> = raw ? JSON.parse(raw) : {};
			return new Set(map[courseId] ?? []);
		} catch {
			return new Set<string>();
		}
	}

	private writeCompleted(courseId: string, ids: Set<string>) {
		try {
			const raw = localStorage.getItem(this.storageKey());
			const map: Record<string, string[]> = raw ? JSON.parse(raw) : {};
			map[courseId] = Array.from(ids);
			localStorage.setItem(this.storageKey(), JSON.stringify(map));
		} catch {
			// localStorage may be unavailable (private mode); completion is
			// non-critical, so silently skip persistence.
		}
	}
}
