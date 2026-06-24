import { ChangeDetectorRef, Component, inject, OnInit, DestroyRef , ChangeDetectionStrategy } from '@angular/core';
import { patchState } from '@ngrx/signals';
import { LoaderModalStore } from '../../stores/loader.store';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { NoticeModalStore } from '../../stores/notice.store';

import {
	FormBuilder,
	FormGroup,
	FormsModule,
	ReactiveFormsModule,
	Validators,
} from '@angular/forms';
import { CourseOptionService } from '../course-option.service';
import { Course, CourseOption } from '../../core/models/course.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DecimalPipe } from '@angular/common';
import { matchesSearch } from '../../shared/utils/search.util';
import { CourseAgencyService } from '../course-agency.service';
import { UserStore } from '../../stores/user.store';

@Component({
    selector: 'app-course-option-list',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [FormsModule, ReactiveFormsModule, DecimalPipe],
    templateUrl: './course-option-list.component.html',
    styleUrl: './course-option-list.component.scss'
})
export class CourseOptionListComponent implements OnInit {
	courseOptions: CourseOption[] = [];
	courseOptionTemps: CourseOption[] = [];
	courses: Course[] = [];

	// store
	loaderStore = inject(LoaderModalStore);
	noticeStore = inject(NoticeModalStore);
	errorModalStore = inject(ErrorModalStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	// add
	isShowAddCourseOptionModal: boolean = false;
	addCourseOptionForm!: FormGroup;

	// delete
	courseOptionIdToDelete: string = '';
	isShowDeleteModal: boolean = false;

	// update
	updateCourseOptionForm!: FormGroup;
	isShowUpdateCourseOptionModal: boolean = false;

	// search
	searchString: string = '';
	selectedCourseFilter: string = '';

	userStore = inject(UserStore);

	constructor(
		private courseOptionService: CourseOptionService,
		private courseAgencyService: CourseAgencyService,
		private formBuilder: FormBuilder
	) {}

	ngOnInit() {
		this.addCourseOptionForm = this.formBuilder.group({
			optionId: [''],
			courseId: ['', [Validators.required]],
			optionName: ['abc', [Validators.required]],
			description: ['abc', [Validators.required]],
			price: [1000000, [Validators.required]],
		});

		this.updateCourseOptionForm = this.formBuilder.group({
			optionId: [''],
			courseId: ['', [Validators.required]],
			optionName: ['', [Validators.required]],
			description: ['', [Validators.required]],
			price: [0, [Validators.required]],
		});

		this.loadCourses();
		this.loadOptions();
	}

	private loadCourses() {
		const trainerId = this.userStore.trainerId() ?? '';
		if (!trainerId) {
			return;
		}
		this.courseAgencyService
			.getCoursesOfTrainer(trainerId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.courses = res;
					if (!this.addCourseOptionForm.controls['courseId'].value && res.length > 0) {
						this.addCourseOptionForm.controls['courseId'].setValue(res[0].courseId);
					}
					this.cdr.markForCheck();
				},
			});
	}

	private loadOptions() {
		patchState(this.loaderStore, { isShow: true });
		this.courseOptionService.getAllCourseOptions().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (res: CourseOption[]) => {
				this.courseOptions = res;
				this.applyFilters();
				patchState(this.loaderStore, { isShow: false });
				this.cdr.markForCheck();
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				patchState(this.errorModalStore, { errors: err.error?.errors, isShow: true });
			},
		});
	}

	onShowDeleteModel(flag: boolean, courseOptionIdToDelete: string) {
		this.isShowDeleteModal = flag;
		this.courseOptionIdToDelete = courseOptionIdToDelete;
	}

	onRemove() {
		this.isShowDeleteModal = false;
		patchState(this.loaderStore, { isShow: true });

		this.courseOptionService
			.removeCourseOptionOfTrainer(this.courseOptionIdToDelete)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: (_res) => {
					patchState(this.loaderStore, { isShow: false });
					patchState(this.noticeStore, { isShow: true, message: 'Deleted successfully' });

					const index = this.courseOptions.findIndex(
						(x) => x.optionId === this.courseOptionIdToDelete
					);
					if (index !== -1) {
						this.courseOptions.splice(index, 1);
					}
					this.courseOptionIdToDelete = '';
					this.applyFilters();
					this.cdr.markForCheck();
				},
				error: err => {
					patchState(this.loaderStore, { isShow: false });
					patchState(this.errorModalStore, { errors: err.error.errors, isShow: true });
				},
			});
	}

	search() {
		this.applyFilters();
	}

	onCourseFilterChange() {
		this.applyFilters();
	}

	private applyFilters() {
		let result = this.courseOptions;
		if (this.selectedCourseFilter) {
			result = result.filter(c => c.courseId === this.selectedCourseFilter);
		}
		if (this.searchString.trim()) {
			result = result.filter(c =>
				matchesSearch(this.searchString, [
					c.optionName,
					c.description,
					this.getCourseTitle(c.courseId),
				])
			);
		}
		this.courseOptionTemps = result;
	}

	onShowAddModal(flag: boolean) {
		this.isShowAddCourseOptionModal = flag;
		if (flag === true) {
			this.addCourseOptionForm.controls['courseId'].setValue(
				this.selectedCourseFilter || this.courses[0]?.courseId || ''
			);
		}
		if (flag === false) {
			this.resetAddForm();
		}
	}

	addCourseOptionSubmit() {
		const optionId = crypto.randomUUID();

		if (this.addCourseOptionForm.valid === false) {
			return;
		} else if (this.addCourseOptionForm.controls['courseId'].value === '') {
			return;
		} else if (this.addCourseOptionForm.controls['optionName'].value === '') {
			return;
		} else if (this.addCourseOptionForm.controls['description'].value === '') {
			return;
		} else if (this.addCourseOptionForm.controls['price'].value <= 0) {
			return;
		}

		this.addCourseOptionForm.controls['optionId'].setValue(optionId);
		const model: CourseOption = { ...this.addCourseOptionForm.value };

		patchState(this.loaderStore, { isShow: true });
		this.courseOptionService.addCourseOptionOfTrainer(model).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (_res) => {
				patchState(this.loaderStore, { isShow: false });
				this.courseOptions.push(model);
				this.applyFilters();
				this.isShowAddCourseOptionModal = false;
				this.resetAddForm();
				this.cdr.markForCheck();
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				patchState(this.errorModalStore, { errors: err.error.errors, isShow: true });
			},
		});
	}

	onShowUpdateModal(flag: boolean, option: CourseOption | null) {
		this.isShowUpdateCourseOptionModal = flag;

		if (flag === false || option === null) {
			this.resetUpdateForm();
		} else {
			this.updateCourseOptionForm.controls['optionId'].setValue(option.optionId);
			this.updateCourseOptionForm.controls['courseId'].setValue(option.courseId);
			this.updateCourseOptionForm.controls['optionName'].setValue(option.optionName);
			this.updateCourseOptionForm.controls['description'].setValue(option.description);
			this.updateCourseOptionForm.controls['price'].setValue(option.price);
		}
	}

	updateCourseOptionSubmit() {
		if (this.updateCourseOptionForm.valid === false) {
			return;
		} else if (this.updateCourseOptionForm.controls['courseId'].value === '') {
			return;
		} else if (this.updateCourseOptionForm.controls['optionName'].value === '') {
			return;
		} else if (this.updateCourseOptionForm.controls['description'].value === '') {
			return;
		} else if (this.updateCourseOptionForm.controls['price'].value <= 0) {
			return;
		} else if (this.updateCourseOptionForm.controls['optionId'].value === '') {
			return;
		}

		const model: CourseOption = { ...this.updateCourseOptionForm.value };
		patchState(this.loaderStore, { isShow: true });

		this.courseOptionService.updateCourseOptionOfTrainer(model).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (_res) => {
				patchState(this.loaderStore, { isShow: false });
				this.isShowUpdateCourseOptionModal = false;

				const option = this.courseOptionTemps.find(
					(x) =>
						x.optionId === this.updateCourseOptionForm.controls['optionId'].value
				);
				if (option) {
					option.courseId = this.updateCourseOptionForm.controls['courseId'].value;
					option.optionName = this.updateCourseOptionForm.controls['optionName'].value;
					option.description = this.updateCourseOptionForm.controls['description'].value;
					option.price = this.updateCourseOptionForm.controls['price'].value;
				}
				this.applyFilters();
				this.resetUpdateForm();
				this.cdr.markForCheck();
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				patchState(this.errorModalStore, { errors: err.error.errors, isShow: true });
			},
		});
	}

	private resetAddForm() {
		this.addCourseOptionForm.controls['optionId'].setValue('');
		this.addCourseOptionForm.controls['courseId'].setValue(this.courses[0]?.courseId || '');
		this.addCourseOptionForm.controls['optionName'].setValue('');
		this.addCourseOptionForm.controls['description'].setValue('');
		this.addCourseOptionForm.controls['price'].setValue(0);
	}

	private resetUpdateForm() {
		this.updateCourseOptionForm.controls['optionId'].setValue('');
		this.updateCourseOptionForm.controls['courseId'].setValue('');
		this.updateCourseOptionForm.controls['optionName'].setValue('');
		this.updateCourseOptionForm.controls['description'].setValue('');
		this.updateCourseOptionForm.controls['price'].setValue(0);
	}

	getCourseTitle(courseId: string): string {
		return this.courses.find(c => c.courseId === courseId)?.title || 'Unassigned course';
	}
}
