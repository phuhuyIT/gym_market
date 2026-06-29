import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { ClassBooking, ClassSession, UpsertClassSession } from '../../core/models/class-schedule.model';
import { ClassScheduleService } from '../../core/services/class-schedule.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';

interface ClassSessionForm {
	title: string;
	description: string;
	startsAt: string;
	endsAt: string;
	capacity: number;
	location: string;
	status: string;
}

@Component({
	selector: 'app-class-schedule',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, FormsModule],
	templateUrl: './class-schedule.component.html',
})
export class ClassScheduleComponent implements OnInit {
	sessions: ClassSession[] = [];
	bookings: ClassBooking[] = [];
	editingId: string | null = null;
	selectedSessionId = '';
	includeCancelled = true;
	form: ClassSessionForm = this.emptyForm();

	private loaderStore = inject(LoaderModalStore);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(private classScheduleService: ClassScheduleService) {}

	ngOnInit() {
		this.loadSessions();
	}

	loadSessions(showLoader = true) {
		if (showLoader) patchState(this.loaderStore, { isShow: true });
		this.classScheduleService
			.getManageSessions(this.includeCancelled)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: sessions => {
					this.sessions = sessions;
					if (!this.selectedSessionId && sessions.length) {
						this.selectedSessionId = sessions[0].classSessionId;
					}
					patchState(this.loaderStore, { isShow: false });
					this.loadBookings(false);
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load class sessions', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	loadBookings(showLoader = true) {
		if (!this.selectedSessionId) {
			this.bookings = [];
			this.cdr.markForCheck();
			return;
		}

		if (showLoader) patchState(this.loaderStore, { isShow: true });
		this.classScheduleService
			.getBookings(this.selectedSessionId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: bookings => {
					this.bookings = bookings;
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load bookings', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	selectSession(session: ClassSession) {
		this.selectedSessionId = session.classSessionId;
		this.loadBookings();
	}

	edit(session: ClassSession) {
		this.editingId = session.classSessionId;
		this.form = {
			title: session.title,
			description: session.description ?? '',
			startsAt: this.toInputValue(session.startsAt),
			endsAt: this.toInputValue(session.endsAt),
			capacity: session.capacity,
			location: session.location ?? '',
			status: session.status,
		};
		this.selectedSessionId = session.classSessionId;
		this.loadBookings(false);
		this.cdr.markForCheck();
	}

	cancelEdit() {
		this.editingId = null;
		this.form = this.emptyForm();
		this.cdr.markForCheck();
	}

	save() {
		const model = this.normalizedForm();
		if (!model) return;

		patchState(this.loaderStore, { isShow: true });
		const request = this.editingId
			? this.classScheduleService.updateSession(this.editingId, model)
			: this.classScheduleService.createSession(model);

		request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: saved => {
				this.sessions = this.editingId
					? this.sessions.map(session => session.classSessionId === saved.classSessionId ? saved : session)
					: [saved, ...this.sessions];
				this.selectedSessionId = saved.classSessionId;
				this.cancelEdit();
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Class session saved');
				this.loadBookings(false);
				this.cdr.markForCheck();
			},
			error: () => {
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Failed to save class session', 'error');
				this.cdr.markForCheck();
			},
		});
	}

	cancelSession(session: ClassSession) {
		patchState(this.loaderStore, { isShow: true });
		this.classScheduleService
			.cancelSession(session.classSessionId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.sessions = this.sessions.map(item =>
						item.classSessionId === session.classSessionId ? { ...item, status: 'Cancelled', bookedCount: 0, availableSpots: item.capacity } : item
					);
					if (this.selectedSessionId === session.classSessionId) this.loadBookings(false);
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Class session cancelled');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to cancel class session', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	markAttendance(booking: ClassBooking, status: 'Attended' | 'NoShow') {
		patchState(this.loaderStore, { isShow: true });
		this.classScheduleService
			.markAttendance(booking.bookingId, status)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: updated => {
					this.bookings = this.bookings.map(item => item.bookingId === updated.bookingId ? updated : item);
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show(status === 'Attended' ? 'Attendance marked' : 'No-show marked');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to update attendance', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	statusClass(status: string): string {
		if (status === 'Scheduled' || status === 'Booked' || status === 'Attended') return 'bg-emerald-500/10 text-emerald-600';
		if (status === 'Cancelled' || status === 'NoShow') return 'bg-red-500/10 text-red-500';
		return 'bg-neutral-500/10 text-neutral-500';
	}

	private normalizedForm(): UpsertClassSession | null {
		const startsAt = new Date(this.form.startsAt);
		const endsAt = new Date(this.form.endsAt);
		const model: UpsertClassSession = {
			title: this.form.title.trim(),
			description: this.form.description.trim() || null,
			startsAt: startsAt.toISOString(),
			endsAt: endsAt.toISOString(),
			capacity: Number(this.form.capacity || 0),
			location: this.form.location.trim() || null,
			status: this.form.status || 'Scheduled',
		};

		if (!model.title) {
			this.toastService.show('Class title is required', 'error');
			return null;
		}

		if (Number.isNaN(startsAt.getTime()) || Number.isNaN(endsAt.getTime()) || endsAt <= startsAt) {
			this.toastService.show('Choose a valid class time', 'error');
			return null;
		}

		if (model.capacity <= 0) {
			this.toastService.show('Capacity must be greater than zero', 'error');
			return null;
		}

		return model;
	}

	private emptyForm(): ClassSessionForm {
		const startsAt = new Date(Date.now() + 86400000);
		startsAt.setMinutes(0, 0, 0);
		const endsAt = new Date(startsAt.getTime() + 3600000);
		return {
			title: '',
			description: '',
			startsAt: this.toInputValue(startsAt),
			endsAt: this.toInputValue(endsAt),
			capacity: 12,
			location: 'Studio A',
			status: 'Scheduled',
		};
	}

	private toInputValue(value: string | Date): string {
		const date = value instanceof Date ? value : new Date(value);
		const local = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
		return local.toISOString().slice(0, 16);
	}
}
