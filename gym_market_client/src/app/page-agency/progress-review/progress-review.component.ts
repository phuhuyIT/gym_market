import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { ProgressGoal, ProgressLog, ProgressSummary } from '../../core/models/progress.model';
import { ProgressService } from '../../core/services/progress.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-progress-review',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule],
	templateUrl: './progress-review.component.html',
})
export class ProgressReviewComponent implements OnInit {
	summaries: ProgressSummary[] = [];
	selectedStudentId = '';
	selectedLogs: ProgressLog[] = [];
	selectedGoal: ProgressGoal | null = null;

	private loaderStore = inject(LoaderModalStore);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(private progressService: ProgressService) {}

	ngOnInit() {
		this.loadSummaries();
	}

	loadSummaries() {
		patchState(this.loaderStore, { isShow: true });
		this.progressService
			.getSummaries()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: summaries => {
					this.summaries = summaries;
					this.selectedStudentId ||= summaries[0]?.studentId ?? '';
					patchState(this.loaderStore, { isShow: false });
					this.loadSelected(false);
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load progress summaries', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	selectStudent(summary: ProgressSummary) {
		this.selectedStudentId = summary.studentId;
		this.loadSelected();
	}

	loadSelected(showLoader = true) {
		if (!this.selectedStudentId) {
			this.selectedLogs = [];
			this.selectedGoal = null;
			this.cdr.markForCheck();
			return;
		}

		if (showLoader) patchState(this.loaderStore, { isShow: true });
		this.progressService
			.getStudentLogs(this.selectedStudentId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: logs => {
					this.selectedLogs = logs;
					this.loadSelectedGoal(false);
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load student progress logs', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	loadSelectedGoal(showLoader = true) {
		if (showLoader) patchState(this.loaderStore, { isShow: true });
		this.progressService
			.getStudentGoal(this.selectedStudentId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: goal => {
					this.selectedGoal = goal;
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load student goal', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	selectedSummary(): ProgressSummary | null {
		return this.summaries.find(summary => summary.studentId === this.selectedStudentId) ?? null;
	}

	statusClass(label: string): string {
		if (label === 'On track') return 'bg-emerald-500/10 text-emerald-600';
		if (label === 'Needs review') return 'bg-red-500/10 text-red-500';
		if (label === 'Waiting for check-in') return 'bg-blue-500/10 text-[#007AFF]';
		return 'bg-neutral-500/10 text-neutral-500';
	}
}
