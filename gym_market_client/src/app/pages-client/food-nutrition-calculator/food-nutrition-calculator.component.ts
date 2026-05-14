import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { UserStore } from '../../stores/user.store';
import { NoticeModalStore } from '../../stores/notice.store';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { LoaderModalStore } from '../../stores/loader.store';
import { FoodNutritionService } from '../../pages-client/food-nutrition.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
	CaloricValueDto,
	FoodNutrition,
	FoodNutritionUser,
} from '../../core/models/food-nutrition.model';
import { NgFor, NgIf } from '@angular/common';

@Component({
	selector: 'app-food-nutrition-calculator',
	standalone: true,
	imports: [ReactiveFormsModule, NgIf, NgFor],
	templateUrl: './food-nutrition-calculator.component.html',
	styleUrl: './food-nutrition-calculator.component.scss',
})
export class FoodNutritionCalculatorComponent implements OnInit {
	showAddFood: boolean = false;
	searchInput: FormControl = new FormControl('');
	weight: FormControl = new FormControl(0);
	foods: FoodNutrition[] = [];
	selectedFood: FoodNutrition | null = null;
	isManualSelection = false;

	userStore = inject(UserStore);
	notice = inject(NoticeModalStore);
	errorModal = inject(ErrorModalStore);
	loader = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);

	foodNutritionUsers: FoodNutritionUser[] = [];

	totalCaloricValue: number = 0;
	totalFat: number = 0;
	totalSugar: number = 0;
	totalProtein: number = 0;

	showDelete: boolean = false;
	foodSelectedToDelete: FoodNutritionUser | null = null;

	constructor(private foodNutritionService: FoodNutritionService) {}

	ngOnInit() {
		this.getFoodNutritionUser();

		this.searchInput.valueChanges
			.pipe(debounceTime(500), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
			.subscribe(value => {
				patchState(this.loader, { isShow: true });
				if (this.isManualSelection) {
					this.isManualSelection = false;
					this.foods = [];
					patchState(this.loader, { isShow: false });
					return;
				}
				this.foodNutritionService.search(value).subscribe({
					next: res => {
						this.foods = res;
						patchState(this.loader, { isShow: false });
					},
					error: () => {
						patchState(this.loader, { isShow: false });
					},
				});
			});
	}

	private getFoodNutritionUser() {
		const userId = this.userStore.id();
		if (userId) {
			this.foodNutritionService
				.getFoodNutritionUser(userId)
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: res => {
						this.foodNutritionUsers = res;
						this.calStatistics();
					},
				});
		}
	}

	calStatistics() {
		this.totalCaloricValue = this.foodNutritionUsers.reduce(
			(sum, food) => sum + (food.caloricValue ?? 0),
			0
		);
		this.totalFat = this.foodNutritionUsers.reduce(
			(sum, food) => sum + (food.fat ?? 0),
			0
		);
		this.totalSugar = this.foodNutritionUsers.reduce(
			(sum, food) => sum + (food.sugars ?? 0),
			0
		);
		this.totalProtein = this.foodNutritionUsers.reduce(
			(sum, food) => sum + (food.protein ?? 0),
			0
		);
	}

	onSelectFood(food: FoodNutrition) {
		this.isManualSelection = true;
		this.selectedFood = food;
		this.searchInput.setValue(food.name);
		this.foods = [];
	}

	onShowAddFoodNutrition(flag: boolean) {
		this.showAddFood = flag;
		this.resetSearchInput();
	}

	onCal() {
		if (!this.selectedFood) return;

		const model: CaloricValueDto = {
			userId: this.userStore.id(),
			foodNutritionId: this.selectedFood.id,
			weight: this.weight.value,
			foodName: this.selectedFood.name,
		};

		patchState(this.loader, { isShow: true });
		this.foodNutritionService
			.calCaloricValue(model)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.showAddFood = false;
					patchState(this.loader, { isShow: false });
					this.foodNutritionUsers.push(res);
					patchState(this.notice, { isShow: true, message: 'Added successfully' });
					this.calStatistics();
				},
				error: () => {
					patchState(this.errorModal, {
						isShow: true,
						errors: ['An error occurred. Please try again.'],
					});
					patchState(this.loader, { isShow: false });
					this.showAddFood = false;
				},
			});
	}

	resetSearchInput() {
		this.selectedFood = null;
		this.searchInput.setValue('');
		this.weight.setValue(0);
	}

	onShowDelete(flag: boolean, food: FoodNutritionUser | null) {
		this.showDelete = flag;
		this.foodSelectedToDelete = food;
	}

	onDelete() {
		if (!this.foodSelectedToDelete) return;

		this.foodNutritionService
			.deleteFoodNutritionUser({
				userId: this.userStore.id() ?? '',
				foodNutritionUserId: this.foodSelectedToDelete.id,
			})
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.notice, { isShow: true, message: 'Deleted successfully' });
					this.showDelete = false;

					this.foodNutritionUsers = this.foodNutritionUsers.filter(
						x => x.id !== this.foodSelectedToDelete?.id
					);
					this.calStatistics();
					this.foodSelectedToDelete = null;
				},
			});
	}
}
