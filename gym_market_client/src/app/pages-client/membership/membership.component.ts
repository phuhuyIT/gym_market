import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { MembershipPlan, MembershipStatus } from '../../core/models/membership.model';
import { MembershipService } from '../../core/services/membership.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-membership',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule],
	templateUrl: './membership.component.html',
})
export class MembershipComponent implements OnInit {
	status: MembershipStatus | null = null;
	selectedPlanId = '';

	private loaderStore = inject(LoaderModalStore);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(private membershipService: MembershipService) {}

	ngOnInit() {
		this.loadStatus();
	}

	loadStatus() {
		patchState(this.loaderStore, { isShow: true });
		this.membershipService
			.getMyStatus()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: status => {
					this.status = status;
					this.selectedPlanId = status.availablePlans[0]?.planId ?? '';
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load membership status', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	selectPlan(plan: MembershipPlan) {
		this.selectedPlanId = plan.planId;
		this.cdr.markForCheck();
	}

	subscribe(plan?: MembershipPlan) {
		const planId = plan?.planId ?? this.selectedPlanId;
		if (!planId) {
			this.toastService.show('Choose a membership plan first', 'error');
			return;
		}

		patchState(this.loaderStore, { isShow: true });
		this.membershipService
			.subscribe(planId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.toastService.show(this.status?.hasActiveMembership ? 'Membership renewed' : 'Membership activated');
					this.loadStatus();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to activate membership', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	cancelMembership() {
		patchState(this.loaderStore, { isShow: true });
		this.membershipService
			.cancelMine()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.toastService.show('Membership cancelled');
					this.loadStatus();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to cancel membership', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	daysRemaining(): number {
		const endsAt = this.status?.currentMembership?.endsAt;
		if (!endsAt) return 0;
		const diff = new Date(endsAt).getTime() - Date.now();
		return Math.max(0, Math.ceil(diff / 86400000));
	}
}
