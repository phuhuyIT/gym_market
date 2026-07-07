import {
	ChangeDetectionStrategy,
	ChangeDetectorRef,
	Component,
	DestroyRef,
	HostListener,
	inject,
	OnInit,
} from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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
	imports: [CommonModule, FormsModule, RouterLink, GmButtonComponent, GmCardComponent],
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
	quizMultiAnswers: Record<string, string[]> = {};
	quizTextAnswers: Record<string, string> = {};
	quizStartedAt: string | null = null;
	honorCodeAccepted = false;
	private proctoringSignalCounts: Record<string, number> = {};
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
						this.lectures = this.sortLectures(res);
						if (this.lectures.length > 0) {
							this.selectLecture(this.lectures.find(lecture => !lecture.isLocked) ?? this.lectures[0]);
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
		this.quizMultiAnswers = {};
		this.quizTextAnswers = {};
		this.courseMaterialService
			.getQuiz(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: quiz => {
					this.quiz = quiz;
					this.latestQuizAttempt = quiz.latestAttempt ?? null;
					this.quizStartedAt = new Date().toISOString();
					this.honorCodeAccepted = false;
					this.proctoringSignalCounts = {};
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
		if (lecture.isLocked) {
			this.selectedLecture = lecture;
			this.materials = [];
			this.cdr.markForCheck();
			return;
		}

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

	selectPreviousLecture() {
		if (this.lectures.length === 0) return;
		const currentIndex = this.selectedLecture
			? this.lectures.findIndex(lecture => lecture.lectureId === this.selectedLecture?.lectureId)
			: 0;
		const previous = this.lectures[Math.max(0, currentIndex - 1)];
		if (previous) {
			this.selectLecture(previous);
		}
	}

	selectNextLecture() {
		if (this.lectures.length === 0) return;
		const currentIndex = this.selectedLecture
			? this.lectures.findIndex(lecture => lecture.lectureId === this.selectedLecture?.lectureId)
			: -1;
		const next = this.lectures[Math.min(this.lectures.length - 1, currentIndex + 1)];
		if (next) {
			this.selectLecture(next);
		}
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
		if (lecture.isLocked) {
			this.toastService.show(lecture.lockReason || 'This lesson is locked', 'error');
			return;
		}

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

	groupedLectures(): { moduleId: string; title: string; lectures: Lecture[] }[] {
		const groups = new Map<string, { moduleId: string; title: string; lectures: Lecture[] }>();
		for (const lecture of this.lectures) {
			const moduleId = lecture.moduleId || 'unassigned';
			const title = lecture.moduleTitle || 'Unassigned lessons';
			if (!groups.has(moduleId)) {
				groups.set(moduleId, { moduleId, title, lectures: [] });
			}
			groups.get(moduleId)!.lectures.push(lecture);
		}
		return Array.from(groups.values());
	}

	lockLabel(lecture: Lecture): string {
		if (!lecture.isLocked) return '';
		if (lecture.unlocksAt) {
			const date = new Date(lecture.unlocksAt);
			if (!Number.isNaN(date.getTime())) {
				return `${lecture.lockReason || 'Unlocks'} ${date.toLocaleString()}`;
			}
		}
		return lecture.lockReason || 'Locked';
	}

	lectureButtonLabel(lecture: Lecture): string {
		const state = lecture.isLocked
			? this.lockLabel(lecture)
			: this.isCompleted(lecture.lectureId)
				? 'completed'
				: 'not completed';
		return `${lecture.title}, ${lecture.activityType || 'Lesson'}, ${state}`;
	}

	completionButtonLabel(lecture: Lecture): string {
		if (lecture.isLocked) return this.lockLabel(lecture);
		return this.isCompleted(lecture.lectureId)
			? `Mark ${lecture.title} as not complete`
			: `Mark ${lecture.title} as complete`;
	}

	private sortLectures(lectures: Lecture[]): Lecture[] {
		return lectures.sort((a, b) =>
			(a.moduleOrder ?? 9999) - (b.moduleOrder ?? 9999) || (a.order ?? 0) - (b.order ?? 0)
		);
	}

	goToDetails() {
		this.router.navigate(['/client/course-details', this.courseId]);
	}

	selectQuizAnswer(questionId: string, optionId: string) {
		this.quizAnswers[questionId] = optionId;
	}

	toggleQuizOption(questionId: string, optionId: string) {
		const selected = this.quizMultiAnswers[questionId] ?? [];
		this.quizMultiAnswers[questionId] = selected.includes(optionId)
			? selected.filter(id => id !== optionId)
			: [...selected, optionId];
	}

	isQuizOptionSelected(questionId: string, optionId: string): boolean {
		return (this.quizMultiAnswers[questionId] ?? []).includes(optionId);
	}

	@HostListener('window:blur')
	onQuizWindowBlur() {
		this.recordProctoringSignal('focus_lost');
	}

	@HostListener('document:visibilitychange')
	onQuizVisibilityChange() {
		if (document.hidden) {
			this.recordProctoringSignal('tab_hidden');
		}
	}

	@HostListener('document:paste')
	onQuizPaste() {
		this.recordProctoringSignal('paste');
	}

	@HostListener('document:fullscreenchange')
	onQuizFullscreenChange() {
		if (!document.fullscreenElement) {
			this.recordProctoringSignal('fullscreen_exit');
		}
	}

	submitQuiz() {
		if (!this.quiz || this.isQuizSubmitting) return;
		if (this.quiz.requireHonorCode && !this.honorCodeAccepted) {
			this.toastService.show('Accept the honor code before submitting', 'error');
			return;
		}
		const unanswered = this.quiz.questions.some(question => {
			if (question.questionType === 'OpenText') {
				return !this.quizTextAnswers[question.questionId]?.trim();
			}
			if (question.questionType === 'MultipleChoice') {
				return (this.quizMultiAnswers[question.questionId] ?? []).length === 0;
			}
			return !this.quizAnswers[question.questionId];
		});
		if (unanswered) {
			this.toastService.show('Answer every question before submitting', 'error');
			return;
		}

		this.isQuizSubmitting = true;
		this.courseMaterialService
			.submitQuiz(this.courseId, {
				startedAt: this.quizStartedAt,
				honorCodeAccepted: this.honorCodeAccepted,
				browserFingerprint: this.browserFingerprint(),
				proctoringSignals: this.proctoringSignals(),
				answers: this.quiz.questions.map(question => ({
					questionId: question.questionId,
					selectedOptionId: question.questionType === 'SingleChoice' ? this.quizAnswers[question.questionId] : null,
					selectedOptionIds: question.questionType === 'MultipleChoice'
						? this.quizMultiAnswers[question.questionId] ?? []
						: [],
					textAnswer: question.questionType === 'OpenText'
						? this.quizTextAnswers[question.questionId]
						: null,
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
					this.quizMultiAnswers = {};
					this.quizTextAnswers = {};
					this.quizStartedAt = new Date().toISOString();
					this.honorCodeAccepted = false;
					this.proctoringSignalCounts = {};
					this.isQuizSubmitting = false;
					this.toastService.show(attempt.requiresManualGrading ? 'Submitted for review' : attempt.passed ? 'Quiz passed' : 'Quiz submitted');
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

	private recordProctoringSignal(type: string) {
		if (!this.quiz?.trackProctoringSignals || this.isQuizSubmitting) return;
		this.proctoringSignalCounts[type] = (this.proctoringSignalCounts[type] ?? 0) + 1;
	}

	private proctoringSignals() {
		if (!this.quiz?.trackProctoringSignals) return [];
		return Object.entries(this.proctoringSignalCounts).map(([type, count]) => ({
			type,
			count,
			occurredAt: new Date().toISOString(),
		}));
	}

	private browserFingerprint(): string {
		return [
			navigator.userAgent,
			navigator.language,
			`${screen.width}x${screen.height}`,
			Intl.DateTimeFormat().resolvedOptions().timeZone,
		].join('|').slice(0, 256);
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
