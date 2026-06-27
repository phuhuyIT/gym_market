import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { MembershipPlan, StudentMembership, UpsertMembershipPlan } from '../../core/models/membership.model';
import { MembershipService } from '../../core/services/membership.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-memberships',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, FormsModule],
	templateUrl: './memberships.component.html',
})
export class MembershipsComponent implements OnInit {
	plans: MembershipPlan[] = [];
	subscriptions: StudentMembership[] = [];
	editingId: string | null = null;
	statusFilter = 'Active';
	form: UpsertMembershipPlan = this.emptyForm();

	private loaderStore = inject(LoaderModalStore);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(private membershipService: MembershipService) {}

	ngOnInit() {
		this.loadData();
	}

	loadData() {
		patchState(this.loaderStore, { isShow: true });
		this.membershipService
			.getPlans(true)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: plans => {
					this.plans = plans;
					this.loadSubscriptions(false);
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load membership plans', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	loadSubscriptions(showLoader = true) {
		if (showLoader) patchState(this.loaderStore, { isShow: true });
		this.membershipService
			.getSubscriptions(this.statusFilter)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: subscriptions => {
					this.subscriptions = subscriptions;
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load subscriptions', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	edit(plan: MembershipPlan) {
		this.editingId = plan.planId;
		this.form = {
			name: plan.name,
			description: plan.description ?? '',
			durationDays: plan.durationDays,
			price: plan.price,
			isActive: plan.isActive,
		};
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
			? this.membershipService.updatePlan(this.editingId, model)
			: this.membershipService.createPlan(model);

		request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: saved => {
				this.plans = this.editingId
					? this.plans.map(plan => plan.planId === saved.planId ? saved : plan)
					: [saved, ...this.plans];
				this.cancelEdit();
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Membership plan saved');
				this.cdr.markForCheck();
			},
			error: () => {
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Failed to save membership plan', 'error');
				this.cdr.markForCheck();
			},
		});
	}

	deactivate(plan: MembershipPlan) {
		patchState(this.loaderStore, { isShow: true });
		this.membershipService
			.deactivatePlan(plan.planId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.plans = this.plans.map(item => item.planId === plan.planId ? { ...item, isActive: false } : item);
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Membership plan deactivated');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to deactivate membership plan', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	private normalizedForm(): UpsertMembershipPlan | null {
		const model: UpsertMembershipPlan = {
			name: this.form.name.trim(),
			description: this.form.description?.trim() || null,
			durationDays: Number(this.form.durationDays || 0),
			price: Number(this.form.price || 0),
			isActive: Boolean(this.form.isActive),
		};

		if (!model.name) {
			this.toastService.show('Plan name is required', 'error');
			return null;
		}

		if (model.durationDays <= 0) {
			this.toastService.show('Duration must be greater than zero', 'error');
			return null;
		}

		if (model.price < 0) {
			this.toastService.show('Price cannot be negative', 'error');
			return null;
		}

		return model;
	}

	private emptyForm(): UpsertMembershipPlan {
		return {
			name: '',
			description: '',
			durationDays: 30,
			price: 0,
			isActive: true,
		};
	}
}
