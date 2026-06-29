import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { StudentWorkoutAssignment, WorkoutExercise } from '../../core/models/workout-plan.model';
import { WorkoutPlanService } from '../../core/services/workout-plan.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-my-workouts',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule],
	templateUrl: './my-workouts.component.html',
})
export class MyWorkoutsComponent implements OnInit {
	assignments: StudentWorkoutAssignment[] = [];

	private loaderStore = inject(LoaderModalStore);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(private workoutPlanService: WorkoutPlanService) {}

	ngOnInit() {
		this.loadAssignments();
	}

	loadAssignments() {
		patchState(this.loaderStore, { isShow: true });
		this.workoutPlanService
			.getMyAssignments()
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: assignments => {
					this.assignments = assignments;
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load workout plans', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	completeExercise(assignment: StudentWorkoutAssignment, exercise: WorkoutExercise) {
		if (exercise.isCompleted || assignment.status !== 'Active') return;

		patchState(this.loaderStore, { isShow: true });
		this.workoutPlanService
			.completeExercise(assignment.assignmentId, exercise.exerciseId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: updated => {
					this.assignments = this.assignments.map(item => item.assignmentId === updated.assignmentId ? updated : item);
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show(updated.status === 'Completed' ? 'Workout plan completed' : 'Exercise completed');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to complete exercise', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	statusClass(status: string): string {
		if (status === 'Active') return 'bg-blue-500/10 text-[#007AFF]';
		if (status === 'Completed') return 'bg-emerald-500/10 text-emerald-600';
		if (status === 'Cancelled') return 'bg-red-500/10 text-red-500';
		return 'bg-neutral-500/10 text-neutral-500';
	}

	assignmentSummary(): { active: number; completed: number } {
		return {
			active: this.assignments.filter(item => item.status === 'Active').length,
			completed: this.assignments.filter(item => item.status === 'Completed').length,
		};
	}
}
