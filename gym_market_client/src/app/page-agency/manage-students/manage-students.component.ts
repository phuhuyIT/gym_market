import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { StudentService } from '../../pages-client/student.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';
import { StudentSearch } from '../../core/models/student.model';
import { SEARCH_DEBOUNCE_MS } from '../../utilities/defaults.const';

interface StudentSummary {
	studentId: string;
	userId?: string;
	studentName: string;
	email: string;
	phoneNumber: string;
	healthStatus: string;
	status: string;
	createdAt: string;
}

@Component({
	selector: 'app-manage-students',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, RouterLink, FormsModule, DatePipe],
	templateUrl: './manage-students.component.html'
})
export class ManageStudentsComponent implements OnInit {
	allStudents: StudentSummary[] = [];
	filteredStudents: StudentSummary[] = [];

	searchString = '';
	statusFilter = '';
	pageIndex = 1;
	pageSize = 15;
	totalCount = 0;
	totalPages = 0;
	hasPreviousPage = false;
	hasNextPage = false;

	loaderStore = inject(LoaderModalStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private studentService = inject(StudentService);
	private searchChanged$ = new Subject<string>();

	ngOnInit() {
		this.searchChanged$
			.pipe(
				debounceTime(SEARCH_DEBOUNCE_MS),
				distinctUntilChanged(),
				takeUntilDestroyed(this.destroyRef)
			)
			.subscribe(() => {
				this.pageIndex = 1;
				this.loadData();
			});

		this.loadData();
	}

	loadData() {
		patchState(this.loaderStore, { isShow: true });
		this.studentService
			.searchStudentsPaged(this.searchString, this.pageIndex, this.pageSize)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: result => {
					this.allStudents = result.items.map(student => this.toSummary(student));
					this.totalCount = result.totalCount;
					this.totalPages = result.totalPages;
					this.hasPreviousPage = result.hasPreviousPage;
					this.hasNextPage = result.hasNextPage;
					this.applyFilters();
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load students', 'error');
				}
			});
	}

	private toSummary(student: StudentSearch): StudentSummary {
		return {
			studentId: student.studentId,
			userId: student.userId,
			studentName: student.fullName || student.name || 'Student',
			email: student.email || '',
			phoneNumber: student.phoneNumber || '',
			healthStatus: student.healthStatus || 'Not provided',
			status: student.status || 'Active',
			createdAt: student.createdAt || ''
		};
	}

	applyFilters() {
		this.filteredStudents = this.statusFilter
			? this.allStudents.filter(s => s.status === this.statusFilter)
			: this.allStudents;
		this.cdr.markForCheck();
	}

	onSearch() {
		this.searchChanged$.next(this.searchString);
	}

	onStatusFilter(status: string) {
		this.statusFilter = status;
		this.applyFilters();
	}

	goToPage(pageIndex: number) {
		if (pageIndex < 1 || (this.totalPages && pageIndex > this.totalPages) || pageIndex === this.pageIndex) return;
		this.pageIndex = pageIndex;
		this.loadData();
	}

	get activeStudents(): number {
		return this.allStudents.filter(s => s.status === 'Active').length;
	}

	get inactiveStudents(): number {
		return this.allStudents.filter(s => s.status !== 'Active').length;
	}
}
