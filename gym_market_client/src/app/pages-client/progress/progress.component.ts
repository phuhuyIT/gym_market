import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { ProgressGoal, ProgressLog, UpsertProgressGoal, UpsertProgressLog } from '../../core/models/progress.model';
import { ProgressService } from '../../core/services/progress.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';

interface ProgressLogForm {
	loggedAt: string;
	weightKg: number | null;
	bodyFatPercent: number | null;
	waistCm: number | null;
	chestCm: number | null;
	armCm: number | null;
	hipCm: number | null;
	strengthNotes: string;
	notes: string;
}

interface ProgressGoalForm {
	targetWeightKg: number | null;
	targetBodyFatPercent: number | null;
	goalDate: string;
	notes: string;
}

@Component({
	selector: 'app-progress',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, FormsModule],
	templateUrl: './progress.component.html',
})
export class ProgressComponent implements OnInit {
	logs: ProgressLog[] = [];
	goal: ProgressGoal | null = null;
	logForm: ProgressLogForm = this.emptyLogForm();
	goalForm: ProgressGoalForm = this.emptyGoalForm();

	private loaderStore = inject(LoaderModalStore);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(private progressService: ProgressService) {}

	ngOnInit() {
		this.loadPage();
	}

	loadPage() {
		patchState(this.loaderStore, { isShow: true });
		this.progressService
			.getMyLogs()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: logs => {
					this.logs = logs;
					this.loadGoal(false);
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load progress logs', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	loadGoal(showLoader = true) {
		if (showLoader) patchState(this.loaderStore, { isShow: true });
		this.progressService
			.getMyGoal()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: goal => {
					this.goal = goal;
					this.goalForm = goal ? {
						targetWeightKg: goal.targetWeightKg ?? null,
						targetBodyFatPercent: goal.targetBodyFatPercent ?? null,
						goalDate: goal.goalDate ? this.toDateInput(goal.goalDate) : '',
						notes: goal.notes ?? '',
					} : this.emptyGoalForm();
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load progress goal', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	saveLog() {
		const model = this.normalizedLog();
		if (!model) return;

		patchState(this.loaderStore, { isShow: true });
		this.progressService
			.createMyLog(model)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: log => {
					this.logs = [log, ...this.logs].sort((a, b) => new Date(b.loggedAt).getTime() - new Date(a.loggedAt).getTime());
					this.logForm = this.emptyLogForm();
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Progress check-in saved');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to save progress check-in', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	saveGoal() {
		const model = this.normalizedGoal();
		if (!model) return;

		patchState(this.loaderStore, { isShow: true });
		this.progressService
			.saveMyGoal(model)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: goal => {
					this.goal = goal;
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Progress goal saved');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to save progress goal', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	latestLog(): ProgressLog | null {
		return this.logs[0] ?? null;
	}

	weightChange(): number | null {
		const latest = this.logs[0]?.weightKg;
		const first = this.logs[this.logs.length - 1]?.weightKg;
		if (this.logs.length < 2 || latest == null || first == null) return null;
		return Number((latest - first).toFixed(2));
	}

	bodyFatChange(): number | null {
		const latest = this.logs[0]?.bodyFatPercent;
		const first = this.logs[this.logs.length - 1]?.bodyFatPercent;
		if (this.logs.length < 2 || latest == null || first == null) return null;
		return Number((latest - first).toFixed(2));
	}

	private normalizedLog(): UpsertProgressLog | null {
		const loggedAt = new Date(this.logForm.loggedAt);
		const model: UpsertProgressLog = {
			loggedAt: loggedAt.toISOString(),
			weightKg: this.logForm.weightKg,
			bodyFatPercent: this.logForm.bodyFatPercent,
			waistCm: this.logForm.waistCm,
			chestCm: this.logForm.chestCm,
			armCm: this.logForm.armCm,
			hipCm: this.logForm.hipCm,
			strengthNotes: this.logForm.strengthNotes.trim() || null,
			notes: this.logForm.notes.trim() || null,
		};

		if (Number.isNaN(loggedAt.getTime())) {
			this.toastService.show('Choose a valid check-in date', 'error');
			return null;
		}

		if (model.weightKg == null && model.bodyFatPercent == null && model.waistCm == null && model.chestCm == null
			&& model.armCm == null && model.hipCm == null && !model.strengthNotes && !model.notes) {
			this.toastService.show('Add at least one progress value', 'error');
			return null;
		}

		return model;
	}

	private normalizedGoal(): UpsertProgressGoal | null {
		const model: UpsertProgressGoal = {
			targetWeightKg: this.goalForm.targetWeightKg,
			targetBodyFatPercent: this.goalForm.targetBodyFatPercent,
			goalDate: this.goalForm.goalDate ? new Date(this.goalForm.goalDate).toISOString() : null,
			status: 'Active',
			notes: this.goalForm.notes.trim() || null,
		};

		if (model.targetWeightKg == null && model.targetBodyFatPercent == null) {
			this.toastService.show('Add a target weight or body fat goal', 'error');
			return null;
		}

		return model;
	}

	private emptyLogForm(): ProgressLogForm {
		return {
			loggedAt: this.toDateInput(new Date()),
			weightKg: null,
			bodyFatPercent: null,
			waistCm: null,
			chestCm: null,
			armCm: null,
			hipCm: null,
			strengthNotes: '',
			notes: '',
		};
	}

	private emptyGoalForm(): ProgressGoalForm {
		return {
			targetWeightKg: null,
			targetBodyFatPercent: null,
			goalDate: '',
			notes: '',
		};
	}

	private toDateInput(value: string | Date): string {
		const date = value instanceof Date ? value : new Date(value);
		return date.toISOString().slice(0, 10);
	}
}
