import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CourseAgencyService } from '../course-agency.service';
import { DatePipe, NgFor, NgIf } from '@angular/common';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { NoticeModalStore } from '../../stores/notice.store';
import { FormsModule } from '@angular/forms';
import { UserStore } from '../../stores/user.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Course } from '../../core/models/course.model';

@Component({
	selector: 'app-course-list',
	standalone: true,
	imports: [RouterLink, DatePipe, FormsModule, NgIf, NgFor],
	templateUrl: './course-list.component.html',
	styleUrl: './course-list.component.scss',
})
export class CourseListComponent implements OnInit {
	courses: Course[] = [];
	coursestemp: Course[] = [];
	loaderStore = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);
	isShowDeleteModal: boolean = false;
	courseIdToDelete: string = '';
	errorModalStore = inject(ErrorModalStore);
	noticeStore = inject(NoticeModalStore);
	userStore = inject(UserStore);

	searchString: string = '';

	constructor(private courseAgencyService: CourseAgencyService) {}

	ngOnInit() {
		patchState(this.loaderStore, { isShow: true });

		this.courseAgencyService
			.getCoursesOftrainer(this.userStore.trainerId() ?? '')
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.courses = res;
					this.coursestemp = this.courses;
					patchState(this.loaderStore, { isShow: false });
				},
				error: () => {
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

		this.courseAgencyService
			.removeCourse(this.courseIdToDelete)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.loaderStore, { isShow: false });
					patchState(this.noticeStore, {
						isShow: true,
						message: 'Course removed successfully',
					});

					this.courses = this.courses.filter(x => x.courseId !== this.courseIdToDelete);
					this.coursestemp = [...this.courses];
					this.courseIdToDelete = '';
				},
				error: err => {
					patchState(this.loaderStore, { isShow: false });
					patchState(this.errorModalStore, {
						errors: err.error?.errors || ['Failed to remove course'],
						isShow: true,
					});
				},
			});
	}

	search() {
		if (this.searchString === '') {
			this.coursestemp = this.courses;
			return;
		}
		this.coursestemp = this.courses.filter(c =>
			c.title.toLowerCase().includes(this.searchString.toLowerCase())
		);
	}
}
