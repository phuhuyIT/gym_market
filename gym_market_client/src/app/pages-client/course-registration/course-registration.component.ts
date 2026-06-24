import { ChangeDetectorRef, Component, DestroyRef, inject , ChangeDetectionStrategy, OnInit } from '@angular/core';
import { CourseRegistrationService } from '../course-registration.service';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { UserStore } from '../../stores/user.store';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Course } from '../../core/models/course.model';
import { GmButtonComponent } from '../../shared/components/gm-button/gm-button.component';
import { ACTIVITY_DAYS, RECOMMENDED_COURSES_FALLBACK } from '../../utilities/mock-data.const';
import { DEFAULT_COURSE_THUMBNAIL_URL } from '../../utilities/defaults.const';
import { FallbackSrcDirective } from '../../shared/directives/fallback-src.directive';
import { matchesSearch } from '../../shared/utils/search.util';
import { ToastService } from '../../shared/services/toast.service';
import { coursePaymentErrorMessage } from '../course-payment-error.util';

@Component({
    selector: 'app-course-registration',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink, FormsModule, CommonModule, GmButtonComponent, FallbackSrcDirective],
    templateUrl: './course-registration.component.html',
    styleUrl: './course-registration.component.scss'
})
export class CourseRegistrationComponent implements OnInit {
	courses: Course[] = [];
	searchString: string | null = null;
	courseSearchs: Course[] = [];
	recommendedCourses: Course[] = [];
	activeTab: string = 'overview';
	readonly DEFAULT_COURSE_THUMBNAIL_URL = DEFAULT_COURSE_THUMBNAIL_URL;

	// Activity widget properties
	selectedActivityDay: string = 'Th';
	readonly activityDays = ACTIVITY_DAYS;

	// Calendar widget properties
	calendarDays: { name: string; date: number; active: boolean; dateObj: Date }[] = [];

	loader = inject(LoaderModalStore);
	userStore = inject(UserStore);
	toastService = inject(ToastService);
	destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(
		private courseRegistrationService: CourseRegistrationService,
		private courseService: CourseAgencyService,
		private router: Router
	) {}

	ngOnInit() {
		this.getCouresRegistrations();
		this.getRecommendedCourses();
		this.generateCalendar();
	}

	private getCouresRegistrations() {
		patchState(this.loader, { isShow: true });
		this.courseRegistrationService
			.getCourses()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: data => {
					patchState(this.loader, { isShow: false });
					this.courses = data as unknown as Course[];
					this.courseSearchs = data as unknown as Course[];
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loader, { isShow: false });
					this.cdr.markForCheck();
				}
			});
	}

	private getRecommendedCourses() {
		const fallbacks = RECOMMENDED_COURSES_FALLBACK;

		this.courseService.getCourses(1, 3, '', '').pipe(
			takeUntilDestroyed(this.destroyRef)
		).subscribe({
			next: res => {
				if (res && res.length > 0) {
					const apiCourses = [...res];
					while (apiCourses.length < 3) {
						apiCourses.push(fallbacks[apiCourses.length]);
					}
					this.recommendedCourses = apiCourses.slice(0, 3);
				} else {
					this.recommendedCourses = fallbacks;
				}
				this.cdr.markForCheck();
			},
			error: () => {
				this.recommendedCourses = fallbacks;
				this.cdr.markForCheck();
			}
		});
	}

	searchCourse() {
		if (this.searchString === null || this.searchString.trim() === '') {
			this.courseSearchs = this.courses;
			return;
		}
		this.courseSearchs = this.courses.filter((c: Course) => matchesSearch(this.searchString, [c.title]));
	}

	// Active tab change handler
	setActiveTab(tab: string) {
		this.activeTab = tab;
		this.cdr.markForCheck();
	}

	// Activity chart handler
	selectActivityDay(day: string) {
		this.selectedActivityDay = day;
		this.cdr.markForCheck();
	}

	// Calendar generation
	private generateCalendar() {
		const today = new Date();
		const currentDayOfWeek = today.getDay(); // 0 (Sun) to 6 (Sat)
		const monday = new Date(today);
		const diff = today.getDate() - currentDayOfWeek + (currentDayOfWeek === 0 ? -6 : 1);
		monday.setDate(diff);

		const dayNames = ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su'];
		this.calendarDays = Array.from({ length: 7 }).map((_, index) => {
			const date = new Date(monday);
			date.setDate(monday.getDate() + index);
			const isToday = date.getDate() === today.getDate() && date.getMonth() === today.getMonth();
			return {
				name: dayNames[index],
				date: date.getDate(),
				active: isToday,
				dateObj: date
			};
		});
		this.cdr.markForCheck();
	}

	selectCalendarDay(day: any) {
		this.calendarDays.forEach(d => d.active = false);
		day.active = true;
		this.cdr.markForCheck();
	}

	getCurrentMonthName(): string {
		return new Date().toLocaleString('default', { month: 'long' });
	}

	// Helpers for deterministic course info
	getCourseProgress(courseId: string): number {
		const course = this.courses.find(c => c.courseId === courseId);
		if (course && !this.isPaid(course)) {
			return 0;
		}

		let sum = 0;
		for (let i = 0; i < courseId.length; i++) {
			sum += courseId.charCodeAt(i);
		}
		return 15 + (sum % 76); // deterministic 15% to 91%
	}

	getCourseModules(courseId: string): number {
		let sum = 0;
		for (let i = 0; i < courseId.length; i++) {
			sum += courseId.charCodeAt(i);
		}
		return 1 + (sum % 12); // 1 to 12
	}

	getCourseVideos(courseId: string): number {
		let sum = 0;
		for (let i = 0; i < courseId.length; i++) {
			sum += courseId.charCodeAt(i);
		}
		return 5 + (sum % 40); // 5 to 45
	}

	getCourseThumbnail(course: Course): string {
		return course.getFileDtos?.find(file => file.typeFile === 'IMAGE')?.url || DEFAULT_COURSE_THUMBNAIL_URL;
	}

	isPaid(course: Course): boolean {
		return course.statusPayment === 'Paid';
	}

	isRetryable(course: Course): boolean {
		return course.statusPayment === 'Canceled' || course.statusPayment === 'Expired';
	}

	isOpenPayment(course: Course): boolean {
		return !!course.statusPayment && !this.isPaid(course) && !this.isRetryable(course);
	}

	statusLabel(course: Course): string {
		switch (course.statusPayment) {
			case 'Paid':
				return 'Paid';
			case 'Expired':
				return 'Expired';
			case 'Canceled':
				return 'Canceled';
			case 'Not Started':
			case 'Pending Payment':
			case 'Pending':
				return 'Pending';
			default:
				return 'Pending';
		}
	}

	statusHelp(course: Course): string {
		switch (course.statusPayment) {
			case 'Paid':
				return 'Payment confirmed. You can start learning.';
			case 'Expired':
				return 'Payment window expired and the seat was released.';
			case 'Canceled':
				return 'Payment was canceled. Start a new payment attempt to continue.';
			default:
				return 'Complete payment to unlock this course.';
		}
	}

	statusClass(course: Course): string {
		if (this.isPaid(course)) return 'pay-badge--paid';
		if (course.statusPayment === 'Expired') return 'pay-badge--expired';
		if (course.statusPayment === 'Canceled') return 'pay-badge--canceled';
		return 'pay-badge--pending';
	}

	primaryActionLabel(course: Course): string {
		if (this.isPaid(course)) return 'START LEARNING';
		if (this.isRetryable(course)) return 'RETRY PAYMENT';
		return 'COMPLETE PAYMENT';
	}

	onCourseAction(course: Course) {
		if (this.isPaid(course)) {
			this.router.navigate(['/client/course-learn', course.courseId]);
			return;
		}

		if (this.isRetryable(course)) {
			this.retryPayment(course.courseId);
			return;
		}

		this.router.navigate(['/client/course-payment', course.courseId]);
	}

	retryPayment(courseId: string) {
		patchState(this.loader, { isShow: true });
		this.courseRegistrationService
			.registerCourse(courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.loader, { isShow: false });
					this.toastService.show('Payment restarted. Please complete the new payment.');
					this.router.navigate(['/client/course-payment', courseId]);
				},
				error: err => {
					patchState(this.loader, { isShow: false });
					this.toastService.show(
						coursePaymentErrorMessage(err, 'Could not restart payment for this course.'),
						'error'
					);
					this.cdr.markForCheck();
				},
			});
	}

	getUpcomingEvents(): { title: string; time: string }[] {
		if (this.courses.length === 0) {
			return [
				{ title: 'Fundamentals of UIUX Design', time: '18th Aug, 2026 09:30AM' },
				{ title: 'Fundamentals of UIUX Design', time: '18th Aug, 2026 09:30AM' },
				{ title: 'Fundamentals of UIUX Design', time: '18th Aug, 2026 09:30AM' }
			];
		}
		const days = ['Today', 'Tomorrow', 'Wednesday', 'Thursday', 'Friday'];
		return this.courses.slice(0, 3).map((c, i) => {
			return {
				title: `Fundamentals of ${c.title}`,
				time: `18th Aug, 2026 09:30AM`
			};
		});
	}
}
