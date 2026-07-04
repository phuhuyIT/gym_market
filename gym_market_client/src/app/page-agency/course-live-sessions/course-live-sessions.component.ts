import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CourseCalendarItem, CourseLiveSession, UpsertCourseLiveSession } from '../../core/models/course-live-session.model';
import { CourseLiveSessionService } from '../../core/services/course-live-session.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-course-live-sessions',
	standalone: true,
	imports: [CommonModule, FormsModule, RouterLink],
	templateUrl: './course-live-sessions.component.html',
	styleUrl: './course-live-sessions.component.scss',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CourseLiveSessionsComponent implements OnInit {
	courseId = '';
	sessions: CourseLiveSession[] = [];
	calendarItems: CourseCalendarItem[] = [];
	selected: CourseLiveSession | null = null;
	isLoading = false;
	isSaving = false;

	form = this.emptyForm();

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
			.getForManagement(this.courseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: sessions => {
					this.sessions = sessions;
					if (this.selected) {
						this.selected = sessions.find(session => session.liveSessionId === this.selected?.liveSessionId) ?? null;
					}
					this.loadCalendar();
					this.isLoading = false;
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

	selectSession(session: CourseLiveSession): void {
		this.selected = session;
		this.form = {
			title: session.title,
			description: session.description || '',
			startsAt: this.toLocalInput(session.startsAt),
			endsAt: this.toLocalInput(session.endsAt),
			meetingUrl: session.meetingUrl || '',
			recordingUrl: session.recordingUrl || '',
			status: session.status || 'Draft',
			attendanceRequired: session.attendanceRequired,
		};
	}

	newSession(): void {
		this.selected = null;
		this.form = this.emptyForm();
	}

	save(): void {
		if (this.isSaving) return;
		if (!this.form.title.trim() || !this.form.startsAt || !this.form.endsAt) {
			this.toastService.show('Title and schedule are required', 'error');
			return;
		}

		const model: UpsertCourseLiveSession = {
			title: this.form.title.trim(),
			description: this.form.description.trim() || null,
			startsAt: new Date(this.form.startsAt).toISOString(),
			endsAt: new Date(this.form.endsAt).toISOString(),
			meetingUrl: this.form.meetingUrl.trim() || null,
			recordingUrl: this.form.recordingUrl.trim() || null,
			status: this.form.status,
			attendanceRequired: this.form.attendanceRequired,
		};

		this.isSaving = true;
		const request = this.selected
			? this.liveSessionService.update(this.selected.liveSessionId, model)
			: this.liveSessionService.create(this.courseId, model);

		request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: session => {
				this.isSaving = false;
				this.selected = session;
				this.toastService.show(session.status === 'Draft' ? 'Live session saved' : 'Live session scheduled');
				this.load();
				this.cdr.markForCheck();
			},
			error: err => {
				this.isSaving = false;
				this.toastService.show(err?.error?.message || err?.error?.Message || 'Failed to save live session', 'error');
				this.cdr.markForCheck();
			},
		});
	}

	deleteSession(session: CourseLiveSession): void {
		this.liveSessionService
			.remove(session.liveSessionId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					if (this.selected?.liveSessionId === session.liveSessionId) this.newSession();
					this.toastService.show('Live session deleted');
					this.load();
				},
				error: () => this.toastService.show('Failed to delete live session', 'error'),
			});
	}

	statusClass(status: string): string {
		return `status-${status.toLowerCase()}`;
	}

	itemClass(type: string): string {
		return `item-${type.replace('_', '-')}`;
	}

	formatDate(value: string): string {
		return new Intl.DateTimeFormat(undefined, { month: 'short', day: 'numeric', hour: 'numeric', minute: '2-digit' }).format(new Date(value));
	}

	private emptyForm() {
		const starts = new Date();
		starts.setDate(starts.getDate() + 7);
		starts.setMinutes(0, 0, 0);
		const ends = new Date(starts);
		ends.setHours(ends.getHours() + 1);
		return {
			title: '',
			description: '',
			startsAt: this.toLocalInput(starts.toISOString()),
			endsAt: this.toLocalInput(ends.toISOString()),
			meetingUrl: '',
			recordingUrl: '',
			status: 'Scheduled',
			attendanceRequired: true,
		};
	}

	private toLocalInput(value: string): string {
		const date = new Date(value);
		const offset = date.getTimezoneOffset() * 60000;
		return new Date(date.getTime() - offset).toISOString().slice(0, 16);
	}
}
