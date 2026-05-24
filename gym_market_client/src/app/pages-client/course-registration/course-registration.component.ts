import { ChangeDetectorRef, Component, DestroyRef, inject , ChangeDetectionStrategy, OnInit } from '@angular/core';
import { CourseRegistrationService } from '../course-registration.service';
import { CourseAgencyService } from '../../page-agency/course-agency.service';
import { UserStore } from '../../stores/user.store';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LoaderModalStore } from '../../stores/loader.store';
import { patchState } from '@ngrx/signals';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Course } from '../../core/models/course.model';
import { GmButtonComponent } from '../../shared/components/gm-button/gm-button.component';

@Component({
    selector: 'app-course-registration',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterLink, FormsModule, CommonModule, GmButtonComponent],
    templateUrl: './course-registration.component.html',
    styleUrl: './course-registration.component.scss'
})
export class CourseRegistrationComponent implements OnInit {
	courses: Course[] = [];
	searchString: string | null = null;
	courseSearchs: Course[] = [];
	recommendedCourses: Course[] = [];
	activeTab: string = 'overview';

	// Activity widget properties
	selectedActivityDay: string = 'Th';
	activityDays = [
		{ day: 'S', hours: 1, label: '1hr' },
		{ day: 'M', hours: 1.5, label: '1hr 30m' },
		{ day: 'T', hours: 2.5, label: '2hr 30m' },
		{ day: 'W', hours: 2, label: '2hr' },
		{ day: 'Th', hours: 3.5, label: '3hr 30min' },
		{ day: 'F', hours: 3, label: '3hr' },
		{ day: 'S', hours: 0.8, label: '48m' }
	];

	// Calendar widget properties
	calendarDays: { name: string; date: number; active: boolean; dateObj: Date }[] = [];

	loader = inject(LoaderModalStore);
	userStore = inject(UserStore);
	destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(
		private courseRegistrationService: CourseRegistrationService,
		private courseService: CourseAgencyService
	) {}

	ngOnInit() {
		this.getCouresRegistrations();
		this.getRecommendedCourses();
		this.generateCalendar();
	}

	private getCouresRegistrations() {
		patchState(this.loader, { isShow: true });
		this.courseRegistrationService
			.getCourses(this.userStore.studentId())
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
		const fallbacks: Course[] = [
			{
				courseId: 'rec-1',
				trainerId: 'trainer-1',
				title: 'Fundamental of UIUX Design',
				description: 'Use Figma to get a job in UI design, UX design any where in the world',
				type: 'Design',
				category: 'Design & Creativity',
				price: 99,
				additionalPrice: 0,
				startDate: '',
				endDate: '',
				duration: 8,
				maxParticipants: 100,
				rating: 4.8,
				getFileDtos: [{ fileId: 'f1', courseId: 'rec-1', url: 'https://images.unsplash.com/photo-1581291518633-83b4ebd1d83e?auto=format&fit=crop&w=600&q=80', typeFile: 'IMAGE' }]
			},
			{
				courseId: 'rec-2',
				trainerId: 'trainer-2',
				title: 'Figma UIUX Design Masterclass',
				description: 'Use Figma to get a job in UI design, UX design any where in the world',
				type: 'Design',
				category: 'Data and Technology',
				price: 129,
				additionalPrice: 0,
				startDate: '',
				endDate: '',
				duration: 12,
				maxParticipants: 150,
				rating: 4.5,
				getFileDtos: [{ fileId: 'f2', courseId: 'rec-2', url: 'https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?auto=format&fit=crop&w=600&q=80', typeFile: 'IMAGE' }]
			},
			{
				courseId: 'rec-3',
				trainerId: 'trainer-3',
				title: 'Advanced Graphics & Visual Design',
				description: 'Use Figma to get a job in UI design, UX design any where in the world',
				type: 'Design',
				category: 'Software Development',
				price: 149,
				additionalPrice: 0,
				startDate: '',
				endDate: '',
				duration: 15,
				maxParticipants: 80,
				rating: 4.9,
				getFileDtos: [{ fileId: 'f3', courseId: 'rec-3', url: 'https://images.unsplash.com/photo-1626785774573-4b799315345d?auto=format&fit=crop&w=600&q=80', typeFile: 'IMAGE' }]
			}
		];

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
		this.courseSearchs = this.courses.filter((c: Course) =>
			c.title.toLowerCase().includes(this.searchString!.toLowerCase())
		);
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

