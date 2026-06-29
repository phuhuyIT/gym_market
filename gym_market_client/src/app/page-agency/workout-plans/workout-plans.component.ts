import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { StudentSearch } from '../../core/models/student.model';
import { StudentWorkoutAssignment, UpsertWorkoutExercise, UpsertWorkoutPlan, WorkoutPlan } from '../../core/models/workout-plan.model';
import { WorkoutPlanService } from '../../core/services/workout-plan.service';
import { StudentService } from '../../pages-client/student.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-workout-plans',
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, FormsModule],
	templateUrl: './workout-plans.component.html',
})
export class WorkoutPlansComponent implements OnInit {
	plans: WorkoutPlan[] = [];
	assignments: StudentWorkoutAssignment[] = [];
	students: StudentSearch[] = [];
	editingId: string | null = null;
	includeInactive = true;
	statusFilter = 'Active';
	selectedStudentId = '';
	selectedPlanId = '';
	form: UpsertWorkoutPlan = this.emptyForm();

	private loaderStore = inject(LoaderModalStore);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(
		private workoutPlanService: WorkoutPlanService,
		private studentService: StudentService,
	) {}

	ngOnInit() {
		this.loadData();
	}

	loadData() {
		patchState(this.loaderStore, { isShow: true });
		this.workoutPlanService
			.getPlans(this.includeInactive)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: plans => {
					this.plans = plans;
					this.selectedPlanId ||= plans.find(plan => plan.isActive)?.workoutPlanId ?? '';
					this.loadAssignments(false);
					this.loadStudents(false);
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load workout plans', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	loadAssignments(showLoader = true) {
		if (showLoader) patchState(this.loaderStore, { isShow: true });
		this.workoutPlanService
			.getAssignments(this.statusFilter)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: assignments => {
					this.assignments = assignments;
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load workout assignments', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	loadStudents(showLoader = true) {
		if (showLoader) patchState(this.loaderStore, { isShow: true });
		this.studentService
			.searchStudentsPaged('', 1, 100)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: result => {
					this.students = result.items;
					this.selectedStudentId ||= result.items[0]?.studentId ?? '';
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load students', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	edit(plan: WorkoutPlan) {
		this.editingId = plan.workoutPlanId;
		this.form = {
			name: plan.name,
			goal: plan.goal ?? '',
			difficulty: plan.difficulty,
			durationWeeks: plan.durationWeeks,
			isActive: plan.isActive,
			exercises: plan.exercises.map(exercise => ({
				weekNumber: exercise.weekNumber,
				dayNumber: exercise.dayNumber,
				order: exercise.order,
				name: exercise.name,
				sets: exercise.sets,
				reps: exercise.reps,
				restSeconds: exercise.restSeconds,
				notes: exercise.notes ?? '',
			})),
		};
		this.cdr.markForCheck();
	}

	cancelEdit() {
		this.editingId = null;
		this.form = this.emptyForm();
		this.cdr.markForCheck();
	}

	addExercise() {
		this.form.exercises.push({
			weekNumber: 1,
			dayNumber: 1,
			order: this.form.exercises.length + 1,
			name: '',
			sets: 3,
			reps: '10',
			restSeconds: 60,
			notes: '',
		});
		this.cdr.markForCheck();
	}

	removeExercise(index: number) {
		if (this.form.exercises.length <= 1) {
			this.toastService.show('A workout plan needs at least one exercise', 'error');
			return;
		}
		this.form.exercises.splice(index, 1);
		this.form.exercises.forEach((exercise, exerciseIndex) => exercise.order = exerciseIndex + 1);
		this.cdr.markForCheck();
	}

	save() {
		const model = this.normalizedForm();
		if (!model) return;

		patchState(this.loaderStore, { isShow: true });
		const request = this.editingId
			? this.workoutPlanService.updatePlan(this.editingId, model)
			: this.workoutPlanService.createPlan(model);

		request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: saved => {
				this.plans = this.editingId
					? this.plans.map(plan => plan.workoutPlanId === saved.workoutPlanId ? saved : plan)
					: [saved, ...this.plans];
				this.selectedPlanId ||= saved.workoutPlanId;
				this.cancelEdit();
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Workout plan saved');
				this.cdr.markForCheck();
			},
			error: () => {
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Failed to save workout plan. Assigned plans cannot be edited.', 'error');
				this.cdr.markForCheck();
			},
		});
	}

	deactivate(plan: WorkoutPlan) {
		patchState(this.loaderStore, { isShow: true });
		this.workoutPlanService
			.deactivatePlan(plan.workoutPlanId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.plans = this.plans.map(item => item.workoutPlanId === plan.workoutPlanId ? { ...item, isActive: false } : item);
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Workout plan deactivated');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to deactivate workout plan', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	assignSelectedPlan() {
		if (!this.selectedPlanId || !this.selectedStudentId) {
			this.toastService.show('Choose a plan and a student first', 'error');
			return;
		}

		patchState(this.loaderStore, { isShow: true });
		this.workoutPlanService
			.assignPlan(this.selectedPlanId, this.selectedStudentId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: assignment => {
					this.assignments = [assignment, ...this.assignments];
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Workout plan assigned');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to assign workout plan', 'error');
					this.cdr.markForCheck();
				},
			});
	}

	cancelAssignment(assignment: StudentWorkoutAssignment) {
		patchState(this.loaderStore, { isShow: true });
		this.workoutPlanService
			.cancelAssignment(assignment.assignmentId)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.assignments = this.assignments.map(item => item.assignmentId === assignment.assignmentId ? { ...item, status: 'Cancelled' } : item);
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Assignment cancelled');
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to cancel assignment', 'error');
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

	private normalizedForm(): UpsertWorkoutPlan | null {
		const exercises = this.form.exercises.map((exercise, index) => ({
			weekNumber: Number(exercise.weekNumber || 1),
			dayNumber: Number(exercise.dayNumber || 1),
			order: Number(exercise.order || index + 1),
			name: exercise.name.trim(),
			sets: Number(exercise.sets || 0),
			reps: exercise.reps.trim(),
			restSeconds: Number(exercise.restSeconds || 0),
			notes: exercise.notes?.trim() || null,
		}));

		const model: UpsertWorkoutPlan = {
			name: this.form.name.trim(),
			goal: this.form.goal?.trim() || null,
			difficulty: this.form.difficulty || 'Beginner',
			durationWeeks: Number(this.form.durationWeeks || 0),
			isActive: Boolean(this.form.isActive),
			exercises,
		};

		if (!model.name) {
			this.toastService.show('Plan name is required', 'error');
			return null;
		}

		if (model.durationWeeks <= 0) {
			this.toastService.show('Duration must be greater than zero', 'error');
			return null;
		}

		if (exercises.length === 0 || exercises.some(exercise => !exercise.name || exercise.sets <= 0 || exercise.restSeconds < 0)) {
			this.toastService.show('Check every exercise name, set count, and rest time', 'error');
			return null;
		}

		return model;
	}

	private emptyForm(): UpsertWorkoutPlan {
		return {
			name: '',
			goal: '',
			difficulty: 'Beginner',
			durationWeeks: 4,
			isActive: true,
			exercises: [
				{
					weekNumber: 1,
					dayNumber: 1,
					order: 1,
					name: '',
					sets: 3,
					reps: '10',
					restSeconds: 60,
					notes: '',
				},
			],
		};
	}
}
