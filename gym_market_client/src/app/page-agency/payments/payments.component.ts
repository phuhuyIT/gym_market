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
import { NoticeModalStore } from '../../stores/notice.store';
import { UserStore } from '../../stores/user.store';
import { ToastService } from '../../shared/services/toast.service';
import { Course } from '../../core/models/course.model';
import { CancelPaymentDto, Payment } from '../../core/models/payment.model';

@Component({
    selector: 'app-manage-students',
    standalone: true,
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [CommonModule, RouterLink, FormsModule, DecimalPipe, DatePipe],
    templateUrl: './manage-students.component.html'
})
export class ManageStudentsComponent implements OnInit {
	courses: Course[] = [];
	allStudents: any[] = [];
	filteredStudents: any[] = [];

	searchString = '';
	statusFilter = '';
	
	// cancel modal state
	showCancelModal = false;
	paymentIdToCancel = '';
	cancelNote = '';

	loaderStore = inject(LoaderModalStore);
	noticeStore = inject(NoticeModalStore);
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
					this.loadStudentsFromCourses();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load courses', 'error');
				}
			});
	}

	loadStudentsFromCourses() {
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

		forkJoin(requests).subscribe({
			next: (results) => {
				const list: any[] = [];
				results.forEach((payments, idx) => {
					const course = this.courses[idx];
					payments.forEach(p => {
						list.push({
							paymentId: p.paymentId,
							studentId: p.studentId,
							studentName: p.fullName || 'Student',
							courseId: course.courseId,
							courseTitle: course.title,
							paymentAmount: p.paymentAmount || course.price,
							paymentStatus: p.paymentStatus || 'Paid',
							paymentDate: p.paymentDate || p.createdAt || new Date().toISOString(),
							note: p.note
						});
					});
				});

				// Sort by registration date desc
				list.sort((a, b) => new Date(b.paymentDate).getTime() - new Date(a.paymentDate).getTime());
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
			result = result.filter(s => 
				s.studentName.toLowerCase().includes(q) || 
				s.courseTitle.toLowerCase().includes(q)
			);
		}

		if (this.statusFilter) {
			result = result.filter(s => s.paymentStatus === this.statusFilter);
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

	okPayment(paymentId: string) {
		patchState(this.loaderStore, { isShow: true });
		this.paymentService
			.okPayment(paymentId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Student registration approved');
					
					const student = this.allStudents.find(s => s.paymentId === paymentId);
					if (student) {
						student.paymentStatus = 'Paid';
					}
					this.applyFilters();
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to approve registration', 'error');
				}
			});
	}

	onShowCancelModal(flag: boolean, paymentId: string) {
		this.showCancelModal = flag;
		this.paymentIdToCancel = paymentId;
		this.cancelNote = '';
	}

	cancelPaymentSubmit() {
		if (!this.cancelNote.trim()) {
			this.toastService.show('Please provide a reason/note for cancellation', 'error');
			return;
		}

		this.showCancelModal = false;
		patchState(this.loaderStore, { isShow: true });

		const model: CancelPaymentDto = {
			paymentId: this.paymentIdToCancel,
			note: this.cancelNote
		};

		this.paymentService
			.cancelPayment(model)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Registration canceled');
					
					const student = this.allStudents.find(s => s.paymentId === this.paymentIdToCancel);
					if (student) {
						student.paymentStatus = 'Canceled';
						student.note = this.cancelNote;
					}
					this.applyFilters();
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to cancel registration', 'error');
				}
			});
	}
}
