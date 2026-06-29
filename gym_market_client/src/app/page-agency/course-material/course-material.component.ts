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
import { CourseModule, Lecture, LectureMaterial } from '../../core/models/lecture.model';
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
	modules: CourseModule[] = [];
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
	editingLectureId: string | null = null;

	// module modals
	isShowModuleModal: boolean = false;
	moduleForm!: FormGroup;
	editingModuleId: string | null = null;

	// material modals
	isShowMaterialModal: boolean = false;
	materialForm!: FormGroup;
	private editingMaterialId: string | null = null;

	// delete modal
	isShowDeleteModal: boolean = false;
	private deleteKind: 'module' | 'lecture' | 'material' | null = null;
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

	get editingModuleLabel(): string {
		return this.editingModuleId ? 'Edit module' : 'Add module';
	}

	get editingMaterialLabel(): string {
		return this.editingMaterialId ? 'Edit material' : 'Add material';
	}

	ngOnInit() {
		this.courseId = this.route.snapshot.params['courseId'];

		this.lectureForm = this.formBuilder.group({
			moduleId: [''],
			title: ['', [Validators.required]],
			description: [''],
			activityType: ['Lesson', [Validators.required]],
			order: [1, [Validators.required, Validators.min(1)]],
			duration: [0, [Validators.min(0)]],
			prerequisiteLectureId: [''],
			unlockAfterDays: [null, [Validators.min(0)]],
			availableFrom: [''],
			availableUntil: [''],
			isPreview: [false],
			isPublished: [true],
		});

		this.moduleForm = this.formBuilder.group({
			title: ['', [Validators.required]],
			description: [''],
			order: [1, [Validators.required, Validators.min(1)]],
			prerequisiteModuleId: [''],
			unlockAfterDays: [null, [Validators.min(0)]],
			availableFrom: [''],
			availableUntil: [''],
			isPublished: [true],
		});

		this.materialForm = this.formBuilder.group({
			materialType: ['VIDEO', [Validators.required]],
			materialContent: ['', [Validators.required]],
		});

		this.loadModules();
		this.loadLectures();
		this.loadQuiz();
		this.loadQuizGradebook();
	}

	private loadModules() {
		if (!this.courseId) return;
		this.courseMaterialService
			.getModulesByCourse(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.modules = this.sortModules(res);
					this.cdr.markForCheck();
				},
				error: () => this.toastService.show('Failed to load modules', 'error'),
			});
	}

	private loadLectures() {
		if (!this.courseId) return;
		patchState(this.loaderStore, { isShow: true });
		this.courseMaterialService
			.getLecturesByCourse(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
						this.lectures = this.sortLectures(res);
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

	lecturesForModule(moduleId: string): Lecture[] {
		return this.lectures.filter(lecture => lecture.moduleId === moduleId);
	}

	get unassignedLectures(): Lecture[] {
		return this.lectures.filter(lecture => !lecture.moduleId);
	}

	moduleTitle(moduleId?: string | null): string {
		return this.modules.find(module => module.moduleId === moduleId)?.title || 'Unassigned';
	}

	lectureTitle(lectureId?: string | null): string {
		return this.lectures.find(lecture => lecture.lectureId === lectureId)?.title || 'None';
	}

	private sortModules(modules: CourseModule[]): CourseModule[] {
		return modules.sort((a, b) => (a.order ?? 0) - (b.order ?? 0));
	}

	private sortLectures(lectures: Lecture[]): Lecture[] {
		return lectures.sort((a, b) => {
			const moduleOrderA = a.moduleOrder ?? this.modules.find(m => m.moduleId === a.moduleId)?.order ?? 9999;
			const moduleOrderB = b.moduleOrder ?? this.modules.find(m => m.moduleId === b.moduleId)?.order ?? 9999;
			return moduleOrderA - moduleOrderB || (a.order ?? 0) - (b.order ?? 0);
		});
	}

	// ---------- Module CRUD ----------
	onShowModuleModal(flag: boolean, module: CourseModule | null = null) {
		this.isShowModuleModal = flag;
		if (!flag) {
			this.editingModuleId = null;
			this.moduleForm.reset({
				title: '',
				description: '',
				order: 1,
				prerequisiteModuleId: '',
				unlockAfterDays: null,
				availableFrom: '',
				availableUntil: '',
				isPublished: true,
			});
			return;
		}

		if (module) {
			this.editingModuleId = module.moduleId;
			this.moduleForm.patchValue({
				title: module.title,
				description: module.description ?? '',
				order: module.order,
				prerequisiteModuleId: module.prerequisiteModuleId ?? '',
				unlockAfterDays: module.unlockAfterDays ?? null,
				availableFrom: this.toDateTimeLocal(module.availableFrom),
				availableUntil: this.toDateTimeLocal(module.availableUntil),
				isPublished: module.isPublished,
			});
		} else {
			this.editingModuleId = null;
			this.moduleForm.reset({
				title: '',
				description: '',
				order: this.modules.length + 1,
				prerequisiteModuleId: '',
				unlockAfterDays: null,
				availableFrom: '',
				availableUntil: '',
				isPublished: true,
			});
		}
	}

	submitModule() {
		if (this.moduleForm.invalid) {
			this.toastService.show('Please fill in the module title', 'error');
			return;
		}

		const isEdit = this.editingModuleId !== null;
		const model: CourseModule = {
			moduleId: this.editingModuleId ?? crypto.randomUUID(),
			courseId: this.courseId,
			title: this.moduleForm.controls['title'].value,
			description: this.moduleForm.controls['description'].value || null,
			order: this.moduleForm.controls['order'].value,
			prerequisiteModuleId: this.moduleForm.controls['prerequisiteModuleId'].value || null,
			unlockAfterDays: this.moduleForm.controls['unlockAfterDays'].value ?? null,
			availableFrom: this.moduleForm.controls['availableFrom'].value || null,
			availableUntil: this.moduleForm.controls['availableUntil'].value || null,
			isPublished: !!this.moduleForm.controls['isPublished'].value,
		};

		patchState(this.loaderStore, { isShow: true });
		const request$ = isEdit
			? this.courseMaterialService.updateModule(model)
			: this.courseMaterialService.addModule(model);

		request$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: saved => {
				patchState(this.loaderStore, { isShow: false });
				if (isEdit) {
					const idx = this.modules.findIndex(m => m.moduleId === saved.moduleId);
					if (idx !== -1) this.modules[idx] = saved;
				} else {
					this.modules.push(saved);
				}
				this.modules = this.sortModules(this.modules);
				this.toastService.show(isEdit ? 'Module updated' : 'Module added');
				this.onShowModuleModal(false);
				this.cdr.markForCheck();
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				patchState(this.errorModalStore, { errors: err.error?.errors || err.error?.error, isShow: true });
			},
		});
	}

	// ---------- Lecture CRUD ----------
	onShowLectureModal(flag: boolean, lecture: Lecture | null = null) {
		this.isShowLectureModal = flag;
		if (!flag) {
			this.editingLectureId = null;
			this.lectureForm.reset({
				moduleId: '',
				title: '',
				description: '',
				activityType: 'Lesson',
				order: 1,
				duration: 0,
				prerequisiteLectureId: '',
				unlockAfterDays: null,
				availableFrom: '',
				availableUntil: '',
				isPreview: false,
				isPublished: true,
			});
			return;
		}
		if (lecture) {
			this.editingLectureId = lecture.lectureId;
			this.lectureForm.patchValue({
				moduleId: lecture.moduleId ?? '',
				title: lecture.title,
				description: lecture.description ?? '',
				activityType: lecture.activityType ?? 'Lesson',
				order: lecture.order,
				duration: lecture.duration,
				prerequisiteLectureId: lecture.prerequisiteLectureId ?? '',
				unlockAfterDays: lecture.unlockAfterDays ?? null,
				availableFrom: this.toDateTimeLocal(lecture.availableFrom),
				availableUntil: this.toDateTimeLocal(lecture.availableUntil),
				isPreview: !!lecture.isPreview,
				isPublished: lecture.isPublished !== false,
			});
		} else {
			this.editingLectureId = null;
			this.lectureForm.reset({
				moduleId: '',
				title: '',
				description: '',
				activityType: 'Lesson',
				order: this.lectures.length + 1,
				duration: 0,
				prerequisiteLectureId: '',
				unlockAfterDays: null,
				availableFrom: '',
				availableUntil: '',
				isPreview: false,
				isPublished: true,
			});
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
				moduleId: this.lectureForm.controls['moduleId'].value || null,
				title: this.lectureForm.controls['title'].value,
				description: this.lectureForm.controls['description'].value || null,
				activityType: this.lectureForm.controls['activityType'].value,
				order: this.lectureForm.controls['order'].value,
				duration: this.lectureForm.controls['duration'].value,
				prerequisiteLectureId: this.lectureForm.controls['prerequisiteLectureId'].value || null,
				unlockAfterDays: this.lectureForm.controls['unlockAfterDays'].value ?? null,
				availableFrom: this.lectureForm.controls['availableFrom'].value || null,
				availableUntil: this.lectureForm.controls['availableUntil'].value || null,
				isPreview: !!this.lectureForm.controls['isPreview'].value,
				isPublished: !!this.lectureForm.controls['isPublished'].value,
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
						if (idx !== -1) this.lectures[idx] = { ...model, moduleTitle: this.moduleTitle(model.moduleId), moduleOrder: this.modules.find(m => m.moduleId === model.moduleId)?.order };
						if (this.selectedLecture?.lectureId === model.lectureId)
							this.selectedLecture = this.lectures[idx] ?? model;
					} else {
						this.lectures.push({ ...model, moduleTitle: this.moduleTitle(model.moduleId), moduleOrder: this.modules.find(m => m.moduleId === model.moduleId)?.order });
					}
					this.lectures = this.sortLectures(this.lectures);
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
	onShowDeleteModal(flag: boolean, kind: 'module' | 'lecture' | 'material' | null = null, id: string = '') {
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
			kind === 'module'
				? this.courseMaterialService.removeModule(id)
				: kind === 'lecture'
					? this.courseMaterialService.removeLecture(id)
					: this.courseMaterialService.removeMaterial(id);

		request$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: () => {
				patchState(this.loaderStore, { isShow: false });
					if (kind === 'module') {
						this.modules = this.modules.filter(m => m.moduleId !== id);
						this.lectures = this.lectures.map(lecture =>
							lecture.moduleId === id
								? { ...lecture, moduleId: null, moduleTitle: null, moduleOrder: null }
								: lecture
						);
					} else if (kind === 'lecture') {
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

	private toDateTimeLocal(value?: string | null): string {
		if (!value) return '';
		const date = new Date(value);
		if (Number.isNaN(date.getTime())) return '';
		const pad = (part: number) => part.toString().padStart(2, '0');
		return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
	}
}
