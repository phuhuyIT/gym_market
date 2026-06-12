import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { CourseAgencyService } from '../course-agency.service';
import { PaymentService } from '../payment.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { UserStore } from '../../stores/user.store';
import { ToastService } from '../../shared/services/toast.service';
import { Course } from '../../core/models/course.model';

interface StudentEnrollment {
	courseId: string;
	courseTitle: string;
	status: string;
}

interface StudentSummary {
	studentId: string;
	studentName: string;
	enrollments: StudentEnrollment[];
	courseCount: number;
	activeCount: number;
	pendingCount: number;
	totalPaid: number;
	status: 'Active' | 'Pending' | 'Inactive';
	lastActivity: string;
}

@Component({
    selector: 'app-manage-students',
    standalone: true,
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [CommonModule, RouterLink, FormsModule, DecimalPipe, DatePipe],
    templateUrl: './manage-students.component.html'
})
export class ManageStudentsComponent implements OnInit {
	courses: Course[] = [];
	allStudents: StudentSummary[] = [];
	filteredStudents: StudentSummary[] = [];

	searchString = '';
	statusFilter = '';

	loaderStore = inject(LoaderModalStore);
	userStore = inject(UserStore);
	toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);
	private paymentService = inject(PaymentService);

	constructor(private courseAgencyService: CourseAgencyService) {}

	ngOnInit() {
		this.loadData();
	}

	loadData() {
		patchState(this.loaderStore, { isShow: true });
		this.courseAgencyService
			.getCoursesOfTrainer(this.userStore.trainerId() ?? '')
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: courses => {
					this.courses = courses;
					this.buildStudentsFromCourses();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load courses', 'error');
				}
			});
	}

	buildStudentsFromCourses() {
		if (this.courses.length === 0) {
			this.allStudents = [];
			this.filteredStudents = [];
			patchState(this.loaderStore, { isShow: false });
			this.cdr.markForCheck();
			return;
		}

		const requests = this.courses.map(c =>
			this.paymentService.getPayments(c.courseId).pipe(
				catchError(() => of([]))
			)
		);

		forkJoin(requests).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: (results) => {
				const byStudent = new Map<string, StudentSummary>();

				results.forEach((payments, idx) => {
					const course = this.courses[idx];
					payments.forEach(p => {
						const status = p.paymentStatus || 'Paid';
						const amount = p.paymentAmount || course.price;
						const date = p.paymentDate || p.createdAt || new Date().toISOString();

						let student = byStudent.get(p.studentId);
						if (!student) {
							student = {
								studentId: p.studentId,
								studentName: p.fullName || 'Student',
								enrollments: [],
								courseCount: 0,
								activeCount: 0,
								pendingCount: 0,
								totalPaid: 0,
								status: 'Inactive',
								lastActivity: date
							};
							byStudent.set(p.studentId, student);
						}

						student.enrollments.push({ courseId: course.courseId, courseTitle: course.title, status });
						if (status === 'Paid') {
							student.activeCount++;
							student.totalPaid += amount;
						} else if (status === 'Pending') {
							student.pendingCount++;
						}
						if (new Date(date).getTime() > new Date(student.lastActivity).getTime()) {
							student.lastActivity = date;
						}
					});
				});

				const list = Array.from(byStudent.values());
				list.forEach(s => {
					s.courseCount = s.enrollments.length;
					s.status = s.activeCount > 0 ? 'Active' : s.pendingCount > 0 ? 'Pending' : 'Inactive';
				});

				// Most recently active students first
				list.sort((a, b) => new Date(b.lastActivity).getTime() - new Date(a.lastActivity).getTime());
				this.allStudents = list;
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

	applyFilters() {
		let result = this.allStudents;

		if (this.searchString.trim()) {
			const q = this.searchString.toLowerCase();
			result = result.filter(s => s.studentName.toLowerCase().includes(q));
		}

		if (this.statusFilter) {
			result = result.filter(s => s.status === this.statusFilter);
		}

		this.filteredStudents = result;
		this.cdr.markForCheck();
	}

	onSearch() {
		this.applyFilters();
	}

	onStatusFilter(status: string) {
		this.statusFilter = status;
		this.applyFilters();
	}

	courseTitles(student: StudentSummary): string {
		return student.enrollments.map(e => e.courseTitle).join(', ');
	}

	get activeStudents(): number {
		return this.allStudents.filter(s => s.status === 'Active').length;
	}

	get pendingStudents(): number {
		return this.allStudents.filter(s => s.status === 'Pending').length;
	}

	get totalRevenue(): number {
		return this.allStudents.reduce((sum, s) => sum + s.totalPaid, 0);
	}
}
