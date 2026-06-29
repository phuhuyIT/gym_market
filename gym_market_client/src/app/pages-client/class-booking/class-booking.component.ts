import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { ClassSession } from '../../core/models/class-schedule.model';
import { ClassScheduleService } from '../../core/services/class-schedule.service';
import { MembershipStatus } from '../../core/models/membership.model';
import { MembershipService } from '../../core/services/membership.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-class-booking',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, RouterLink],
	templateUrl: './class-booking.component.html',
})
export class ClassBookingComponent implements OnInit {
	sessions: ClassSession[] = [];
	status: MembershipStatus | null = null;

	private loaderStore = inject(LoaderModalStore);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(
		private classScheduleService: ClassScheduleService,
		private membershipService: MembershipService,
	) {}

	ngOnInit() {
		this.loadPage();
	}

	loadPage() {
		patchState(this.loaderStore, { isShow: true });
		this.membershipService
			.getMyStatus()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: status => {
					this.status = status;
					this.loadSessions(false);
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load membership status', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	loadSessions(showLoader = true) {
		if (showLoader) patchState(this.loaderStore, { isShow: true });
		const from = new Date();
		const to = new Date(Date.now() + 30 * 86400000);
		this.classScheduleService
			.getSessions(from, to)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: sessions => {
					this.sessions = sessions;
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load class schedule', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	book(session: ClassSession) {
		if (!this.status?.hasActiveMembership) {
			this.toastService.show('Activate a membership before booking classes', 'error');
			return;
		}

		patchState(this.loaderStore, { isShow: true });
		this.classScheduleService
			.bookSession(session.classSessionId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: booking => {
					this.sessions = this.sessions.map(item =>
						item.classSessionId === session.classSessionId
							? {
									...item,
									isBooked: true,
									myBookingId: booking.bookingId,
									myBookingStatus: booking.status,
									bookedCount: item.bookedCount + 1,
									availableSpots: Math.max(0, item.availableSpots - 1),
								}
							: item
					);
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Class booked');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to book class', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	cancel(session: ClassSession) {
		if (!session.myBookingId) return;

		patchState(this.loaderStore, { isShow: true });
		this.classScheduleService
			.cancelBooking(session.myBookingId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.sessions = this.sessions.map(item =>
						item.classSessionId === session.classSessionId
							? {
									...item,
									isBooked: false,
									myBookingStatus: 'Cancelled',
									bookedCount: Math.max(0, item.bookedCount - 1),
									availableSpots: item.availableSpots + 1,
								}
							: item
					);
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Booking cancelled');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to cancel booking', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	canBook(session: ClassSession): boolean {
		return session.status === 'Scheduled'
			&& !session.isBooked
			&& session.availableSpots > 0
			&& new Date(session.startsAt).getTime() > Date.now();
	}

	statusClass(session: ClassSession): string {
		if (session.isBooked) return 'bg-emerald-500/10 text-emerald-600';
		if (session.availableSpots <= 0) return 'bg-red-500/10 text-red-500';
		if (session.status === 'Cancelled') return 'bg-neutral-500/10 text-neutral-500';
		return 'bg-blue-500/10 text-[#007AFF]';
	}

	sessionLabel(session: ClassSession): string {
		if (session.isBooked) return 'Booked';
		if (session.availableSpots <= 0) return 'Full';
		return session.status;
	}
}
