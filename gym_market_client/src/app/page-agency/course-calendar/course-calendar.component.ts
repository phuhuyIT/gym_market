import { CommonModule, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CourseCalendarItem } from '../../core/models/course-live-session.model';
import { CourseLiveSessionService } from '../../core/services/course-live-session.service';
import { ToastService } from '../../shared/services/toast.service';

type CalendarFilter = 'all' | 'assignment' | 'quiz' | 'live_session' | 'announcement';

@Component({
	selector: 'app-agency-course-calendar',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, FormsModule, RouterLink, DatePipe],
	templateUrl: './course-calendar.component.html',
	styleUrl: './course-calendar.component.scss',
})
export class CourseCalendarComponent implements OnInit {
	items: CourseCalendarItem[] = [];
	filter: CalendarFilter = 'all';
	from = this.toInputDate(this.shiftDate(-14));
	to = this.toInputDate(this.shiftDate(90));
	isLoading = false;

	readonly filters: { value: CalendarFilter; label: string }[] = [
		{ value: 'all', label: 'All' },
		{ value: 'assignment', label: 'Assignments' },
		{ value: 'quiz', label: 'Quizzes' },
		{ value: 'live_session', label: 'Live' },
		{ value: 'announcement', label: 'Announcements' },
	];

	private liveSessionService = inject(CourseLiveSessionService);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit(): void {
		this.load();
	}

	load(): void {
		this.isLoading = true;
		this.liveSessionService
			.getTrainerCalendar(this.fromDate, this.toDate)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: items => {
					this.items = items;
					this.isLoading = false;
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load course calendar', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	setFilter(filter: CalendarFilter): void {
		this.filter = filter;
	}

	get filteredItems(): CourseCalendarItem[] {
		if (this.filter === 'all') return this.items;
		return this.items.filter(item => item.type === this.filter);
	}

	get groupedItems(): { day: string; items: CourseCalendarItem[] }[] {
		const groups = new Map<string, CourseCalendarItem[]>();
		for (const item of this.filteredItems) {
			const day = new Date(item.startsAt).toDateString();
			groups.set(day, [...(groups.get(day) ?? []), item]);
		}
		return Array.from(groups.entries()).map(([day, items]) => ({ day, items }));
	}

	get draftCount(): number {
		return this.items.filter(item => item.status === 'Draft').length;
	}

	get liveCount(): number {
		return this.items.filter(item => item.type === 'live_session').length;
	}

	get dueSoonCount(): number {
		const now = Date.now();
		const soon = now + 48 * 60 * 60 * 1000;
		return this.items.filter(item => {
			const startsAt = new Date(item.startsAt).getTime();
			return startsAt >= now && startsAt <= soon && (item.type === 'assignment' || item.type === 'quiz' || item.type === 'live_session');
		}).length;
	}

	itemClass(type: string): string {
		return `calendar-item--${type.replace('_', '-')}`;
	}

	typeLabel(type: string): string {
		return type.replace('_', ' ');
	}

	private get fromDate(): Date {
		return new Date(`${this.from}T00:00:00`);
	}

	private get toDate(): Date {
		return new Date(`${this.to}T23:59:59`);
	}

	private shiftDate(days: number): Date {
		const date = new Date();
		date.setDate(date.getDate() + days);
		return date;
	}

	private toInputDate(date: Date): string {
		return date.toISOString().slice(0, 10);
	}
}
