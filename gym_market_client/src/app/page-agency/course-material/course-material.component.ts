import {
	ChangeDetectionStrategy,
	ChangeDetectorRef,
	Component,
	DestroyRef,
	inject,
	OnInit,
} from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import {
	FormBuilder,
	FormGroup,
	FormsModule,
	ReactiveFormsModule,
	Validators,
} from '@angular/forms';
import { patchState } from '@ngrx/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LoaderModalStore } from '../../stores/loader.store';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { ToastService } from '../../shared/services/toast.service';
import { CourseMaterialService } from '../course-material.service';
import { Lecture, LectureMaterial } from '../../core/models/lecture.model';
import { QuizAttemptSummary, TrainerQuizQuestion, UpsertCourseQuiz } from '../../core/models/quiz.model';

@Component({
	selector: 'app-course-material',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [FormsModule, ReactiveFormsModule, RouterLink],
	templateUrl: './course-material.component.html',
	styleUrl: './course-material.component.scss',
})
export class CourseMaterialComponent implements OnInit {
	courseId: string = '';

	lectures: Lecture[] = [];
	selectedLecture: Lecture | null = null;
	materials: LectureMaterial[] = [];

	// material types offered to the trainer
	materialTypes: string[] = ['VIDEO', 'IMAGE', 'PDF', 'TEXT', 'LINK'];

	quizDraft: UpsertCourseQuiz = this.createEmptyQuizDraft();
	quizGradebook: QuizAttemptSummary[] = [];
	isQuizLoading = false;

	// lecture modals
	isShowLectureModal: boolean = false;
	lectureForm!: FormGroup;
	private editingLectureId: string | null = null;

	// material modals
	isShowMaterialModal: boolean = false;
	materialForm!: FormGroup;
	private editingMaterialId: string | null = null;

	// delete modal
	isShowDeleteModal: boolean = false;
	private deleteKind: 'lecture' | 'material' | null = null;
	private deleteId: string = '';

	loaderStore = inject(LoaderModalStore);
	errorModalStore = inject(ErrorModalStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private route = inject(ActivatedRoute);
	private formBuilder = inject(FormBuilder);
	private courseMaterialService = inject(CourseMaterialService);

	get editingLectureLabel(): string {
		return this.editingLectureId ? 'Edit lecture' : 'Add lecture';
	}

	get editingMaterialLabel(): string {
		return this.editingMaterialId ? 'Edit material' : 'Add material';
	}

	ngOnInit() {
		this.courseId = this.route.snapshot.params['courseId'];

		this.lectureForm = this.formBuilder.group({
			title: ['', [Validators.required]],
			order: [1, [Validators.required, Validators.min(1)]],
			duration: [0, [Validators.min(0)]],
		});

		this.materialForm = this.formBuilder.group({
			materialType: ['VIDEO', [Validators.required]],
			materialContent: ['', [Validators.required]],
		});

		this.loadLectures();
		this.loadQuiz();
		this.loadQuizGradebook();
	}

	private loadLectures() {
		if (!this.courseId) return;
		patchState(this.loaderStore, { isShow: true });
		this.courseMaterialService
			.getLecturesByCourse(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.lectures = res.sort((a, b) => (a.order ?? 0) - (b.order ?? 0));
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load lectures', 'error');
				},
			});
	}

	selectLecture(lecture: Lecture) {
		this.selectedLecture = lecture;
		this.materials = [];
		patchState(this.loaderStore, { isShow: true });
		this.courseMaterialService
			.getMaterialsByLecture(lecture.lectureId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.materials = res;
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load materials', 'error');
				},
			});
	}

	// ---------- Lecture CRUD ----------
	onShowLectureModal(flag: boolean, lecture: Lecture | null = null) {
		this.isShowLectureModal = flag;
		if (!flag) {
			this.editingLectureId = null;
			this.lectureForm.reset({ title: '', order: 1, duration: 0 });
			return;
		}
		if (lecture) {
			this.editingLectureId = lecture.lectureId;
			this.lectureForm.patchValue({
				title: lecture.title,
				order: lecture.order,
				duration: lecture.duration,
			});
		} else {
			this.editingLectureId = null;
			this.lectureForm.reset({ title: '', order: this.lectures.length + 1, duration: 0 });
		}
	}

	submitLecture() {
		if (this.lectureForm.invalid) {
			this.toastService.show('Please fill in the lecture title', 'error');
			return;
		}

		const isEdit = this.editingLectureId !== null;
		const model: Lecture = {
			lectureId: this.editingLectureId ?? crypto.randomUUID(),
			courseId: this.courseId,
			title: this.lectureForm.controls['title'].value,
			order: this.lectureForm.controls['order'].value,
			duration: this.lectureForm.controls['duration'].value,
		};

		patchState(this.loaderStore, { isShow: true });
		const request$ = isEdit
			? this.courseMaterialService.updateLecture(model)
			: this.courseMaterialService.addLecture(model);

		request$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: () => {
				patchState(this.loaderStore, { isShow: false });
				if (isEdit) {
					const idx = this.lectures.findIndex(l => l.lectureId === model.lectureId);
					if (idx !== -1) this.lectures[idx] = model;
					if (this.selectedLecture?.lectureId === model.lectureId)
						this.selectedLecture = model;
				} else {
					this.lectures.push(model);
				}
				this.lectures.sort((a, b) => (a.order ?? 0) - (b.order ?? 0));
				this.toastService.show(isEdit ? 'Lecture updated' : 'Lecture added');
				this.onShowLectureModal(false);
				this.cdr.markForCheck();
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				patchState(this.errorModalStore, { errors: err.error?.errors, isShow: true });
			},
		});
	}

	// ---------- Material CRUD ----------
	onShowMaterialModal(flag: boolean, material: LectureMaterial | null = null) {
		this.isShowMaterialModal = flag;
		if (!flag) {
			this.editingMaterialId = null;
			this.materialForm.reset({ materialType: 'VIDEO', materialContent: '' });
			return;
		}
		if (material) {
			this.editingMaterialId = material.materialId;
			this.materialForm.patchValue({
				materialType: material.materialType,
				materialContent: material.materialContent,
			});
		} else {
			this.editingMaterialId = null;
			this.materialForm.reset({ materialType: 'VIDEO', materialContent: '' });
		}
	}

	submitMaterial() {
		if (!this.selectedLecture) return;
		if (this.materialForm.invalid) {
			this.toastService.show('Please provide material content', 'error');
			return;
		}

		const isEdit = this.editingMaterialId !== null;
		const model: LectureMaterial = {
			materialId: this.editingMaterialId ?? crypto.randomUUID(),
			lectureId: this.selectedLecture.lectureId,
			materialType: this.materialForm.controls['materialType'].value,
			materialContent: this.materialForm.controls['materialContent'].value,
		};

		patchState(this.loaderStore, { isShow: true });
		const request$ = isEdit
			? this.courseMaterialService.updateMaterial(model)
			: this.courseMaterialService.addMaterial(model);

		request$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: () => {
				patchState(this.loaderStore, { isShow: false });
				if (isEdit) {
					const idx = this.materials.findIndex(m => m.materialId === model.materialId);
					if (idx !== -1) this.materials[idx] = model;
				} else {
					this.materials.push(model);
				}
				this.toastService.show(isEdit ? 'Material updated' : 'Material added');
				this.onShowMaterialModal(false);
				this.cdr.markForCheck();
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				patchState(this.errorModalStore, { errors: err.error?.errors, isShow: true });
			},
		});
	}

	// ---------- Delete ----------
	onShowDeleteModal(flag: boolean, kind: 'lecture' | 'material' | null = null, id: string = '') {
		this.isShowDeleteModal = flag;
		this.deleteKind = kind;
		this.deleteId = id;
	}

	onConfirmDelete() {
		const kind = this.deleteKind;
		const id = this.deleteId;
		this.onShowDeleteModal(false);
		if (!kind || !id) return;

		patchState(this.loaderStore, { isShow: true });
		const request$ =
			kind === 'lecture'
				? this.courseMaterialService.removeLecture(id)
				: this.courseMaterialService.removeMaterial(id);

		request$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: () => {
				patchState(this.loaderStore, { isShow: false });
				if (kind === 'lecture') {
					this.lectures = this.lectures.filter(l => l.lectureId !== id);
					if (this.selectedLecture?.lectureId === id) {
						this.selectedLecture = null;
						this.materials = [];
					}
				} else {
					this.materials = this.materials.filter(m => m.materialId !== id);
				}
				this.toastService.show('Deleted successfully');
				this.cdr.markForCheck();
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				patchState(this.errorModalStore, { errors: err.error?.errors, isShow: true });
			},
		});
	}

	private loadQuiz() {
		if (!this.courseId) return;
		this.isQuizLoading = true;
		this.courseMaterialService
			.getManageQuiz(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: quiz => {
					this.quizDraft = {
						title: quiz.title,
						passingScorePercent: quiz.passingScorePercent,
						isPublished: quiz.isPublished,
						questions: quiz.questions.map(question => ({
							questionId: question.questionId,
							prompt: question.prompt,
							order: question.order,
							points: question.points,
							options: question.options.map(option => ({
								optionId: option.optionId,
								text: option.text,
								isCorrect: option.isCorrect,
							})),
						})),
					};
					this.isQuizLoading = false;
					this.cdr.markForCheck();
				},
				error: err => {
					this.isQuizLoading = false;
					if (err.status !== 404) {
						this.toastService.show('Failed to load quiz', 'error');
					}
					this.cdr.markForCheck();
				},
			});
	}

	private loadQuizGradebook() {
		if (!this.courseId) return;
		this.courseMaterialService
			.getQuizGradebook(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: attempts => {
					this.quizGradebook = attempts;
					this.cdr.markForCheck();
				},
				error: err => {
					if (err.status !== 404) {
						this.toastService.show('Failed to load quiz grades', 'error');
					}
				},
			});
	}

	addQuizQuestion() {
		this.quizDraft.questions.push({
			prompt: '',
			order: this.quizDraft.questions.length + 1,
			points: 1,
			options: [
				{ text: '', isCorrect: true },
				{ text: '', isCorrect: false },
			],
		});
		this.cdr.markForCheck();
	}

	removeQuizQuestion(index: number) {
		this.quizDraft.questions.splice(index, 1);
		this.quizDraft.questions.forEach((question, idx) => (question.order = idx + 1));
		this.cdr.markForCheck();
	}

	addQuizOption(question: TrainerQuizQuestion) {
		question.options.push({ text: '', isCorrect: false });
		this.cdr.markForCheck();
	}

	removeQuizOption(question: TrainerQuizQuestion, index: number) {
		if (question.options.length <= 2) {
			this.toastService.show('Each question needs at least two options', 'error');
			return;
		}
		const wasCorrect = question.options[index].isCorrect;
		question.options.splice(index, 1);
		if (wasCorrect && question.options.length > 0) {
			question.options[0].isCorrect = true;
		}
		this.cdr.markForCheck();
	}

	markCorrect(question: TrainerQuizQuestion, optionIndex: number) {
		question.options.forEach((option, index) => (option.isCorrect = index === optionIndex));
	}

	saveQuiz() {
		if (this.quizGradebook.length > 0) {
			this.toastService.show('This quiz already has attempts, so questions are locked', 'error');
			return;
		}

		patchState(this.loaderStore, { isShow: true });
		this.courseMaterialService
			.saveQuiz(this.courseId, this.quizDraft)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: quiz => {
					patchState(this.loaderStore, { isShow: false });
					this.quizDraft = {
						title: quiz.title,
						passingScorePercent: quiz.passingScorePercent,
						isPublished: quiz.isPublished,
						questions: quiz.questions.map(question => ({
							questionId: question.questionId,
							prompt: question.prompt,
							order: question.order,
							points: question.points,
							options: question.options.map(option => ({
								optionId: option.optionId,
								text: option.text,
								isCorrect: option.isCorrect,
							})),
						})),
					};
					this.toastService.show('Quiz saved');
					this.cdr.markForCheck();
				},
				error: err => {
					patchState(this.loaderStore, { isShow: false });
					const message = err.status === 409
						? 'This quiz already has attempts, so questions are locked'
						: 'Failed to save quiz';
					this.toastService.show(message, 'error');
				},
			});
	}

	private createEmptyQuizDraft(): UpsertCourseQuiz {
		return {
			title: 'Course checkpoint',
			passingScorePercent: 70,
			isPublished: false,
			questions: [
				{
					prompt: '',
					order: 1,
					points: 1,
					options: [
						{ text: '', isCorrect: true },
						{ text: '', isCorrect: false },
					],
				},
			],
		};
	}
}
