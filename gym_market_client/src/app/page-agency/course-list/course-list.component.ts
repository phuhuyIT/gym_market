import { Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CourseAgencyService } from '../course-agency.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { FormsModule } from '@angular/forms';
import { UserStore } from '../../stores/user.store';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Course } from '../../core/models/course.model';
import { GmCardComponent, GmButtonComponent, GmInputComponent } from '../../shared';
import { ToastService } from '../../shared/services/toast.service';

@Component({
    selector: 'app-course-list',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink, FormsModule, GmCardComponent, GmButtonComponent, GmInputComponent],
    templateUrl: './course-list.component.html',
    styleUrl: './course-list.component.scss'
})
export class CourseListComponent implements OnInit {
	courses: Course[] = [];
	coursestemp: Course[] = [];
	loaderStore = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);
	isShowDeleteModal: boolean = false;
	courseIdToDelete: string = '';
	userStore = inject(UserStore);
	toastService = inject(ToastService);

	searchString: string = '';

	constructor(private courseAgencyService: CourseAgencyService) {}

	ngOnInit() {
		this.loadCourses();
	}

	loadCourses() {
		patchState(this.loaderStore, { isShow: true });
		this.courseAgencyService
			.getCoursesOfTrainer(this.userStore.trainerId() ?? '')
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
					this.toastService.show('Course removed successfully');
					this.courses = this.courses.filter(x => x.courseId !== this.courseIdToDelete);
					this.coursestemp = [...this.courses];
					this.courseIdToDelete = '';
				},
				error: err => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to remove course', 'error');
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
