import { Component, inject } from '@angular/core';
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
import { AddCourseOption } from '../models/add-course-option.model';

@Component({
	selector: 'app-course-option-list',
	standalone: true,
	imports: [FormsModule, ReactiveFormsModule],
	templateUrl: './course-option-list.component.html',
	styleUrl: './course-option-list.component.scss',
})
export class CourseOptionListComponent {
	courseOptions: any = [];
	courseOptionTemps: any;

	// store
	loaderStore = inject(LoaderModalStore);
	noticeStore = inject(NoticeModalStore);
	errorModalStore = inject(ErrorModalStore);

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

	constructor(
		private courseOptionService: CourseOptionService,
		private formBuilder: FormBuilder
	) {}

	ngOnInit() {
		this.addCourseOptionForm = this.formBuilder.group({
			optionId: [''],
			optionName: ['abc', [Validators.required]],
			description: ['abc', [Validators.required]],
			price: [1000000, [Validators.required]],
		});

		this.updateCourseOptionForm = this.formBuilder.group({
			optionId: [''],
			optionName: ['', [Validators.required]],
			description: ['', [Validators.required]],
			price: [0, [Validators.required]],
		});

		patchState(this.loaderStore, { isShow: true });

		this.courseOptionService.getCourseOptionsOftrainer().subscribe({
			next: (res: any) => {
				// console.log(res);
				this.courseOptions = res;
				this.courseOptionTemps = this.courseOptions;
				patchState(this.loaderStore, { isShow: false });
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
			.removeCourseOptionOftrainer(this.courseOptionIdToDelete)
			.subscribe({
				next: (res: any) => {
					patchState(this.loaderStore, { isShow: false });
					patchState(this.noticeStore, { isShow: true, message: 'Xóa thành công' });

					const index = this.courseOptions.findIndex(
						(x: any) => x.optionId === this.courseOptionIdToDelete
					);
					if (index !== -1) {
						this.courseOptions.splice(index, 1);
					}
					this.courseOptionIdToDelete = '';
				},
				error: err => {
					patchState(this.loaderStore, { isShow: false });
					patchState(this.errorModalStore, { errors: err.error.errors, isShow: true });
				},
			});
	}

	search() {
		if (this.searchString === '') {
			this.courseOptionTemps = this.courseOptions;
			return;
		}
		this.courseOptionTemps = this.courseOptions.filter((c: any) =>
			c.optionName.toLowerCase().includes(this.searchString.toLowerCase())
		);
	}

	onShowAddModal(flag: boolean) {
		this.isShowAddCourseOptionModal = flag;
	}

	addCourseOptionSubmit() {
		const optionId = crypto.randomUUID();

		if (this.addCourseOptionForm.valid === false) {
			return;
		} else if (this.addCourseOptionForm.controls['optionName'].value === '') {
			return;
		} else if (this.addCourseOptionForm.controls['description'].value === '') {
			return;
		} else if (this.addCourseOptionForm.controls['price'].value <= 0) {
			return;
		}

		this.addCourseOptionForm.controls['optionId'].setValue(optionId);
		const model: AddCourseOption = { ...this.addCourseOptionForm.value };

		patchState(this.loaderStore, { isShow: true });
		this.courseOptionService.addCourseOptionOftrainer(model).subscribe({
			next: (res: any) => {
				patchState(this.loaderStore, { isShow: false });
				this.courseOptions.push(model);
				this.courseOptionTemps = this.courseOptions;
				this.isShowAddCourseOptionModal = false;
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				let result = [];
				for (const key in err.error.errors) {
					if (err.error.errors.hasOwnProperty(key)) {
						result.push(`${key}: ${err.error.errors[key][0]}\n`);
					}
				}
			},
		});
	}

	onShowUpdateModal(flag: boolean, option: any) {
		this.isShowUpdateCourseOptionModal = flag;

		if (flag === false) {
			this.updateCourseOptionForm.controls['optionId'].setValue('');
			this.updateCourseOptionForm.controls['optionName'].setValue('');
			this.updateCourseOptionForm.controls['description'].setValue('');
			this.updateCourseOptionForm.controls['price'].setValue(0);
		} else {
			this.updateCourseOptionForm.controls['optionId'].setValue(option.optionId);
			this.updateCourseOptionForm.controls['optionName'].setValue(option.optionName);
			this.updateCourseOptionForm.controls['description'].setValue(option.description);
			this.updateCourseOptionForm.controls['price'].setValue(option.price);
		}
	}

	updateCourseOptionSubmit() {
		if (this.updateCourseOptionForm.valid === false) {
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

		const model: AddCourseOption = { ...this.updateCourseOptionForm.value };
		patchState(this.loaderStore, { isShow: true });

		this.courseOptionService.updateCourseOptionOftrainer(model).subscribe({
			next: (res: any) => {
				patchState(this.loaderStore, { isShow: false });
				this.isShowUpdateCourseOptionModal = false;

				const option = this.courseOptionTemps.find(
					(x: any) =>
						x.optionId === this.updateCourseOptionForm.controls['optionId'].value
				);
				if (option) {
					option.optionName = this.updateCourseOptionForm.controls['optionName'].value;
					option.description = this.updateCourseOptionForm.controls['description'].value;
					option.price = this.updateCourseOptionForm.controls['price'].value;
				}
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				let result = [];
				for (const key in err.error.errors) {
					if (err.error.errors.hasOwnProperty(key)) {
						result.push(`${key}: ${err.error.errors[key][0]}\n`);
					}
				}
			},
		});
	}
}
