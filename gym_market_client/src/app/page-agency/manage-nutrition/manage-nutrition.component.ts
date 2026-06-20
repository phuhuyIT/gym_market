import { ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { patchState } from '@ngrx/signals';
import { FoodNutrition, FoodNutritionUpsertDto } from '../../core/models/food-nutrition.model';
import { FoodNutritionService } from '../../pages-client/food-nutrition.service';
import { LoaderModalStore } from '../../stores/loader.store';
import { ToastService } from '../../shared/services/toast.service';

@Component({
	selector: 'app-manage-nutrition',
	standalone: true,
	changeDetection: ChangeDetectionStrategy.OnPush,
	imports: [CommonModule, FormsModule, DecimalPipe],
	templateUrl: './manage-nutrition.component.html',
})
export class ManageNutritionComponent implements OnInit {
	foods: FoodNutrition[] = [];
	searchString = '';
	editingId: number | null = null;
	deleteTarget: FoodNutrition | null = null;

	form: FoodNutritionUpsertDto = this.emptyForm();

	private loaderStore = inject(LoaderModalStore);
	private toastService = inject(ToastService);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	constructor(private foodNutritionService: FoodNutritionService) {}

	ngOnInit() {
		this.loadFoods();
	}

	loadFoods() {
		patchState(this.loaderStore, { isShow: true });
		this.foodNutritionService
			.search(this.searchString, 1, 100)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: foods => {
					this.foods = foods;
					patchState(this.loaderStore, { isShow: false });
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Failed to load foods', 'error');
				},
			});
	}

	onSearch() {
		this.loadFoods();
	}

	edit(food: FoodNutrition) {
		this.editingId = food.id;
		this.form = {
			name: food.name,
			caloricValue: food.caloricValue,
			carbs: food.carbs ?? 0,
			fat: food.fat,
			sugars: food.sugars,
			protein: food.protein,
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
			? this.foodNutritionService.updateFoodNutrition(this.editingId, model)
			: this.foodNutritionService.createFoodNutrition(model);

		request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
			next: saved => {
				if (this.editingId) {
					this.foods = this.foods.map(food => food.id === saved.id ? saved : food);
				} else {
					this.foods = [saved, ...this.foods];
				}
				this.cancelEdit();
				patchState(this.loaderStore, { isShow: false });
				this.toastService.show('Food saved successfully');
				this.cdr.markForCheck();
			},
			error: err => {
				patchState(this.loaderStore, { isShow: false });
				const message = err.status === 403
					? 'Only admins can manage the food database'
					: 'Failed to save food';
				this.toastService.show(message, 'error');
			},
		});
	}

	confirmDelete(food: FoodNutrition) {
		this.deleteTarget = food;
		this.cdr.markForCheck();
	}

	deleteFood() {
		if (!this.deleteTarget) return;

		const id = this.deleteTarget.id;
		patchState(this.loaderStore, { isShow: true });
		this.foodNutritionService
			.deleteFoodNutrition(id)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					this.foods = this.foods.filter(food => food.id !== id);
					this.deleteTarget = null;
					patchState(this.loaderStore, { isShow: false });
					this.toastService.show('Food deleted successfully');
					this.cdr.markForCheck();
				},
				error: err => {
					patchState(this.loaderStore, { isShow: false });
					const message = err.status === 403
						? 'Only admins can manage the food database'
						: 'Failed to delete food';
					this.toastService.show(message, 'error');
				},
			});
	}

	private normalizedForm(): FoodNutritionUpsertDto | null {
		const model: FoodNutritionUpsertDto = {
			name: this.form.name.trim(),
			caloricValue: Number(this.form.caloricValue || 0),
			carbs: Number(this.form.carbs || 0),
			fat: Number(this.form.fat || 0),
			sugars: Number(this.form.sugars || 0),
			protein: Number(this.form.protein || 0),
		};

		if (!model.name) {
			this.toastService.show('Food name is required', 'error');
			return null;
		}

		if ([model.caloricValue, model.carbs, model.fat, model.sugars, model.protein].some(value => value < 0)) {
			this.toastService.show('Nutrition values cannot be negative', 'error');
			return null;
		}

		return model;
	}

	private emptyForm(): FoodNutritionUpsertDto {
		return {
			name: '',
			caloricValue: 0,
			carbs: 0,
			fat: 0,
			sugars: 0,
			protein: 0,
		};
	}
}
