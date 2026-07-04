import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CourseCalendarItem, CourseLiveSession } from '../../core/models/course-live-session.model';
import { CourseLiveSessionService } from '../../core/services/course-live-session.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-client-course-live-sessions',
	standalone: true,
	imports: [CommonModule, RouterLink],
	templateUrl: './course-live-sessions.component.html',
	styleUrl: './course-live-sessions.component.scss',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CourseLiveSessionsComponent implements OnInit {
	courseId = '';
	sessions: CourseLiveSession[] = [];
	calendarItems: CourseCalendarItem[] = [];
	isLoading = false;

	private route = inject(ActivatedRoute);
	private liveSessionService = inject(CourseLiveSessionService);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	ngOnInit(): void {
		this.courseId = this.route.snapshot.params['courseId'];
		this.load();
	}

	load(): void {
		this.isLoading = true;
		this.liveSessionService
			.getForStudent(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: sessions => {
					this.sessions = sessions;
					this.isLoading = false;
					this.loadCalendar();
					this.cdr.markForCheck();
				},
				error: () => {
					this.isLoading = false;
					this.toastService.show('Failed to load live sessions', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	loadCalendar(): void {
		const from = new Date();
		from.setDate(from.getDate() - 14);
		const to = new Date();
		to.setDate(to.getDate() + 75);
		this.liveSessionService
			.getCalendar(this.courseId, from, to)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: items => {
					this.calendarItems = items;
					this.cdr.markForCheck();
				},
				error: () => this.toastService.show('Failed to load course calendar', 'error'),
			});
	}

	upcomingSessions(): CourseLiveSession[] {
		const now = Date.now();
		return this.sessions.filter(session => new Date(session.endsAt).getTime() >= now && session.status !== 'Cancelled');
	}

	pastSessions(): CourseLiveSession[] {
		const now = Date.now();
		return this.sessions.filter(session => new Date(session.endsAt).getTime() < now || session.status === 'Cancelled');
	}

	formatDate(value: string): string {
		return new Intl.DateTimeFormat(undefined, { weekday: 'short', month: 'short', day: 'numeric', hour: 'numeric', minute: '2-digit' }).format(new Date(value));
	}

	statusClass(status: string): string {
		return `status-${status.toLowerCase()}`;
	}

	itemClass(type: string): string {
		return `item-${type.replace('_', '-')}`;
	}
}
