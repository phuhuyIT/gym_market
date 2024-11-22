import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CourseAgencyService } from '../course-agency.service';
import { DatePipe } from '@angular/common';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { NoticeModalStore } from '../../stores/notice.store';
import { FormsModule } from '@angular/forms';

@Component({
	selector: 'app-course-list',
	standalone: true,
	imports: [RouterLink, DatePipe, FormsModule],
	templateUrl: './course-list.component.html',
	styleUrl: './course-list.component.scss',
})
export class CourseListComponent {
	courses: any = [];
	coursestemp: any;
	loaderStore = inject(LoaderModalStore);
	isShowDeleteModal: boolean = false;
	courseIdToDelete: string = '';
	errorModalStore = inject(ErrorModalStore);
	noticeStore = inject(NoticeModalStore);

	searchString: string = '';

	constructor(private courseAgencyService: CourseAgencyService) {}

	ngOnInit() {
		patchState(this.loaderStore, { isShow: true });

		this.courseAgencyService.getCourses().subscribe({
			next: (res: any) => {
				// console.log(res);
				this.courses = res;
				this.coursestemp = this.courses;
				patchState(this.loaderStore, { isShow: false });
			},
		});
	}

	onShowDeleteModel(flag: boolean, courseIdToDelete: string) {
		this.isShowDeleteModal = flag;
		this.courseIdToDelete = courseIdToDelete;
	}

	onRemove() {
		this.isShowDeleteModal = false;
		patchState(this.loaderStore, { isShow: true });

		this.courseAgencyService.removeCourse(this.courseIdToDelete).subscribe({
			next: (res: any) => {
				patchState(this.loaderStore, { isShow: false });
				patchState(this.noticeStore, { isShow: true, message: 'Xóa thành công' });

				const index = this.courses.findIndex(
					(x: any) => x.courseId === this.courseIdToDelete
				);
				if (index !== -1) {
					this.courses.splice(index, 1);
				}
				this.courseIdToDelete = '';
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				patchState(this.errorModalStore, { errors: err.error.errors, isShow: true });
			},
		});
	}

	search() {
		if (this.searchString === '') {
			this.coursestemp = this.courses;
			return;
		}
		this.coursestemp = this.courses.filter((c: any) =>
			c.title.toLowerCase().includes(this.searchString.toLowerCase())
		);
	}
}
