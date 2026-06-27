import { CommonModule, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize, take } from 'rxjs';
import { ToastService } from '../../shared/services/toast.service';
import { AdminCourse, CourseModerationStatus } from './admin-course.model';
import { AdminCoursesService } from './admin-courses.service';

@Component({
	selector: 'app-admin-courses',
	imports: [CommonModule, FormsModule, RouterLink, DecimalPipe],
	templateUrl: './admin-courses.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminCoursesComponent implements OnInit {
	private adminCoursesService = inject(AdminCoursesService);
	private toastService = inject(ToastService);
	private cdr = inject(ChangeDetectorRef);

	courses: AdminCourse[] = [];
	search = '';
	status = 'PendingReview';
	pageIndex = 1;
	pageSize = 10;
	totalCount = 0;
	totalPages = 0;
	loading = false;
	actionCourseId = '';

	ngOnInit(): void {
		this.loadCourses();
	}

	loadCourses(): void {
		this.loading = true;
		this.adminCoursesService
			.searchCourses({
				search: this.search,
				status: this.status,
				pageIndex: this.pageIndex,
				pageSize: this.pageSize,
			})
			.pipe(
				take(1),
				finalize(() => {
					this.loading = false;
					this.cdr.markForCheck();
				})
			)
			.subscribe({
				next: result => {
					this.courses = result.items;
					this.totalCount = result.totalCount;
					this.totalPages = result.totalPages;
					this.cdr.markForCheck();
				},
				error: err => this.showError(err, 'Failed to load courses'),
			});
	}

	applyFilters(): void {
		this.pageIndex = 1;
		this.loadCourses();
	}

	clearFilters(): void {
		this.search = '';
		this.status = '';
		this.applyFilters();
	}

	nextPage(): void {
		if (this.pageIndex >= this.totalPages) return;
		this.pageIndex++;
		this.loadCourses();
	}

	previousPage(): void {
		if (this.pageIndex <= 1) return;
		this.pageIndex--;
		this.loadCourses();
	}

	setStatus(course: AdminCourse, status: CourseModerationStatus): void {
		this.actionCourseId = course.courseId;
		this.adminCoursesService
			.updateModeration(course.courseId, status)
			.pipe(
				take(1),
				finalize(() => {
					this.actionCourseId = '';
					this.cdr.markForCheck();
				})
			)
			.subscribe({
				next: () => {
					this.toastService.show(`Course marked ${this.statusLabel(status).toLowerCase()}.`, 'success');
					this.loadCourses();
				},
				error: err => this.showError(err, 'Failed to update course'),
			});
	}

	statusLabel(status?: string | null): string {
		switch (status) {
			case 'PendingReview':
				return 'Pending review';
			case 'Published':
				return 'Published';
			case 'Rejected':
				return 'Rejected';
			case 'Archived':
				return 'Archived';
			case 'Draft':
				return 'Draft';
			default:
				return 'Unknown';
		}
	}

	private showError(err: unknown, fallback: string): void {
		const error = err as { error?: { errors?: string[] | string; message?: string } };
		const errors = error.error?.errors;
		const message = Array.isArray(errors) ? errors.join(', ') : errors || error.error?.message || fallback;
		this.toastService.show(message, 'error');
	}
}
