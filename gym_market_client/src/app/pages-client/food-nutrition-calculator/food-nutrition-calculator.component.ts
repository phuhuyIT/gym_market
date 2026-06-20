import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, switchMap, catchError, EMPTY, forkJoin, of } from 'rxjs';
import { UserStore } from '../../stores/user.store';
import { NoticeModalStore } from '../../stores/notice.store';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { LoaderModalStore } from '../../stores/loader.store';
import { FoodNutritionService } from '../../pages-client/food-nutrition.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
	CaloricValueDto,
	CustomFoodNutritionDto,
	FoodNutrition,
	FoodNutritionUser,
	NutritionBudget,
	UpdateFoodNutritionUserDto,
} from '../../core/models/food-nutrition.model';
import { SEARCH_DEBOUNCE_MS } from '../../utilities/defaults.const';
import { STORAGE_KEYS } from '../../utilities/storage-keys.const';
import { RECIPES_LIST, getFoodImage } from '../../utilities/mock-data.const';


@Component({
    selector: 'app-food-nutrition-calculator',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [ReactiveFormsModule, CommonModule],
    templateUrl: './food-nutrition-calculator.component.html',
    styleUrl: './food-nutrition-calculator.component.scss'
})
export class FoodNutritionCalculatorComponent implements OnInit {
	showAddFood: boolean = false;
	searchInput: FormControl = new FormControl('');
	weight: FormControl = new FormControl(0);
	foods: FoodNutrition[] = [];
	selectedFood: FoodNutrition | null = null;
	isManualSelection = false;
	isCustomFoodMode = false;
	customFoodName = new FormControl('');
	customCalories = new FormControl(0);
	customCarbs = new FormControl(0);
	customFat = new FormControl(0);
	customSugars = new FormControl(0);
	customProtein = new FormControl(0);

	userStore = inject(UserStore);
	notice = inject(NoticeModalStore);
	errorModal = inject(ErrorModalStore);
	loader = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	foodNutritionUsers: FoodNutritionUser[] = [];

	totalCaloricValue: number = 0;
	totalCarbs: number = 0;
	totalFat: number = 0;
	totalSugar: number = 0;
	totalProtein: number = 0;

	showDelete: boolean = false;
	foodSelectedToDelete: FoodNutritionUser | null = null;
	showSettings: boolean = false;

		showEdit: boolean = false;
		foodSelectedToEdit: FoodNutritionUser | null = null;
		editWeight: FormControl = new FormControl(0);
		editCalories: FormControl = new FormControl(0);
		editCarbs: FormControl = new FormControl(0);
		editFat: FormControl = new FormControl(0);
		editSugars: FormControl = new FormControl(0);
		editProtein: FormControl = new FormControl(0);
		editMealType: string = 'Breakfast';

	// Redesign Properties
	selectedDateStr: string = '';
	weekDays: { name: string; dateNum: string; fullDate: string; hasLogs: boolean }[] = [];
	selectedMealType: string = 'Breakfast';

	// Target Budgets
	calorieBudget: number = 2000;
	carbsBudget: number = 250;
	fatBudget: number = 65;
	proteinBudget: number = 130;

	// FormControls for editing targets
	calorieCtrl = new FormControl(2000);
	carbsCtrl = new FormControl(250);
	fatCtrl = new FormControl(65);
	proteinCtrl = new FormControl(130);

	// Categorized user logs
	breakfastLogs: FoodNutritionUser[] = [];
	lunchLogs: FoodNutritionUser[] = [];
	dinnerLogs: FoodNutritionUser[] = [];
	snacksLogs: FoodNutritionUser[] = [];
		filteredLogs: FoodNutritionUser[] = [];
		nutritionSummaries: Record<string, number> = {};

	readonly recipesList = RECIPES_LIST;

	constructor(private foodNutritionService: FoodNutritionService) {}

	ngOnInit() {
		this.loadBudgets();
		
		const today = new Date();
			this.selectedDateStr = this.formatDate(today);
			this.generateWeekForDate(today);

			this.loadCurrentWeekData();

		this.searchInput.valueChanges
			.pipe(
				debounceTime(SEARCH_DEBOUNCE_MS),
				distinctUntilChanged(),
					switchMap(value => {
						if (this.isManualSelection) {
							this.isManualSelection = false;
							this.foods = [];
							return EMPTY;
						}
						const query = String(value || '').trim();
						if (query.length < 2) {
							this.foods = [];
							this.cdr.markForCheck();
							return of([]);
						}

						patchState(this.loader, { isShow: true });
						return this.foodNutritionService.search(query).pipe(
							catchError(() => {
								patchState(this.loader, { isShow: false });
								return of([]);
							})
						);
					}),
				takeUntilDestroyed(this.destroyRef)
			)
			.subscribe(res => {
				this.foods = res;
				patchState(this.loader, { isShow: false });
				this.cdr.markForCheck();
			});
	}

		private getFoodNutritionUser() {
			if (this.userStore.id()) {
			this.foodNutritionService
				.getFoodNutritionUser()
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: res => {
						this.foodNutritionUsers = res;
						this.updateFilteredLogs();
						this.cdr.markForCheck();
					},
				});
			}
		}

		private loadCurrentWeekData() {
			if (!this.userStore.id() || this.weekDays.length === 0) return;

			const from = this.weekDays[0].fullDate;
			const to = this.weekDays[this.weekDays.length - 1].fullDate;
			patchState(this.loader, { isShow: true });

			forkJoin({
				logs: this.foodNutritionService.getFoodNutritionUserRange(from, to),
				summaries: this.foodNutritionService.getNutritionSummary(from, to).pipe(catchError(() => of([]))),
			})
				.pipe(takeUntilDestroyed(this.destroyRef))
				.subscribe({
					next: ({ logs, summaries }) => {
						this.foodNutritionUsers = logs;
						this.nutritionSummaries = summaries.reduce<Record<string, number>>((acc, item) => {
							acc[item.date.slice(0, 10)] = item.entryCount;
							return acc;
						}, {});
						this.updateFilteredLogs();
						patchState(this.loader, { isShow: false });
						this.cdr.markForCheck();
					},
					error: () => {
						patchState(this.loader, { isShow: false });
						patchState(this.errorModal, {
							isShow: true,
							errors: ['Failed to load nutrition logs.'],
						});
					},
				});
		}

	// REDESIGN METHODS
	generateWeekForDate(targetDate: Date) {
		const currentDay = targetDate.getDay(); // 0: Sunday, 1: Monday...
		const distanceToMonday = currentDay === 0 ? -6 : 1 - currentDay;
		const monday = new Date(targetDate);
		monday.setDate(targetDate.getDate() + distanceToMonday);

		this.weekDays = [];
		const dayNames = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
		for (let i = 0; i < 7; i++) {
			const d = new Date(monday);
			d.setDate(monday.getDate() + i);
			const dateStr = this.formatDate(d);
			this.weekDays.push({
				name: dayNames[i],
				dateNum: d.getDate().toString().padStart(2, '0'),
				fullDate: dateStr,
				hasLogs: false
			});
		}
	}

	formatDate(date: Date): string {
		const year = date.getFullYear();
		const month = (date.getMonth() + 1).toString().padStart(2, '0');
		const day = date.getDate().toString().padStart(2, '0');
		return `${year}-${month}-${day}`;
	}

	getMetadata(): Record<number, { date: string; mealType: string }> {
		const data = localStorage.getItem(STORAGE_KEYS.nutritionMetadata);
		return data ? JSON.parse(data) : {};
	}

	deleteMetadata(id: number) {
		const metadata = this.getMetadata();
		delete metadata[id];
		localStorage.setItem(STORAGE_KEYS.nutritionMetadata, JSON.stringify(metadata));
	}

	// The backend persists date/mealType on each log; entries created before
	// that existed fall back to the localStorage metadata.
	getLogDate(item: FoodNutritionUser, metadata = this.getMetadata()): string | undefined {
		return item.date ? item.date.slice(0, 10) : metadata[item.id]?.date;
	}

	getLogMealType(item: FoodNutritionUser, metadata = this.getMetadata()): string {
		return item.mealType ?? metadata[item.id]?.mealType ?? 'Breakfast';
	}

	updateFilteredLogs() {
		const metadata = this.getMetadata();
		const meals = ['Breakfast', 'Lunch', 'Dinner', 'Snacks'];
		let updated = false;

		// Distribute legacy logs with no metadata anywhere dynamically
		this.foodNutritionUsers.forEach((item, index) => {
			if (!item.date && !metadata[item.id]) {
				const mealType = meals[index % meals.length];
				metadata[item.id] = {
					date: this.selectedDateStr,
					mealType: mealType
				};
				updated = true;
			}
		});

		if (updated) {
			localStorage.setItem(STORAGE_KEYS.nutritionMetadata, JSON.stringify(metadata));
		}

		// Filter logs for selected date
		this.filteredLogs = this.foodNutritionUsers.filter(
			item => this.getLogDate(item, metadata) === this.selectedDateStr
		);

		// Check dot indicators for week days
			this.weekDays.forEach(day => {
				day.hasLogs = (this.nutritionSummaries[day.fullDate] ?? 0) > 0
					|| this.foodNutritionUsers.some(item => this.getLogDate(item, metadata) === day.fullDate);
			});

		// Distribute logs by meal
		this.breakfastLogs = [];
		this.lunchLogs = [];
		this.dinnerLogs = [];
		this.snacksLogs = [];

		this.filteredLogs.forEach(item => {
			const mealType = this.getLogMealType(item, metadata);
			if (mealType === 'Breakfast') this.breakfastLogs.push(item);
			else if (mealType === 'Lunch') this.lunchLogs.push(item);
			else if (mealType === 'Dinner') this.dinnerLogs.push(item);
			else if (mealType === 'Snacks') this.snacksLogs.push(item);
		});

		this.calStatistics();
	}

	onSelectDate(day: any) {
		this.selectedDateStr = day.fullDate;
		this.updateFilteredLogs();
	}

	onCustomDateSelected(event: Event) {
		const value = (event.target as HTMLInputElement).value;
		if (value) {
			this.selectedDateStr = value;
			const parts = value.split('-');
			const d = new Date(Number(parts[0]), Number(parts[1]) - 1, Number(parts[2]));
				this.generateWeekForDate(d);
				this.loadCurrentWeekData();
				this.cdr.markForCheck();
			}
		}

	calStatistics() {
		this.totalCaloricValue = this.filteredLogs.reduce(
			(sum, food) => sum + (food.caloricValue ?? 0),
			0
		);
		this.totalCarbs = this.filteredLogs.reduce(
			(sum, food) => sum + this.getCarbs(food),
			0
		);
		this.totalFat = this.filteredLogs.reduce(
			(sum, food) => sum + (food.fat ?? 0),
			0
		);
		this.totalSugar = this.filteredLogs.reduce(
			(sum, food) => sum + (food.sugars ?? 0),
			0
		);
		this.totalProtein = this.filteredLogs.reduce(
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

	onShowAddForMeal(mealType: string) {
		this.selectedMealType = mealType;
		this.onShowAddFoodNutrition(true);
	}

	onCal() {
		if (this.isCustomFoodMode) {
			this.onCreateCustomFood();
			return;
		}

			if (!this.selectedFood || Number(this.weight.value || 0) <= 0) {
				patchState(this.errorModal, {
					isShow: true,
					errors: ['Choose a food and enter a weight greater than 0.'],
				});
				return;
			}

		const model: CaloricValueDto = {
			foodNutritionId: this.selectedFood.id,
				weight: Number(this.weight.value || 0),
			foodName: this.selectedFood.name,
			date: this.selectedDateStr,
			mealType: this.selectedMealType,
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
					this.updateFilteredLogs();
					this.cdr.markForCheck();
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

	onCreateCustomFood() {
		const foodName = (this.customFoodName.value || this.searchInput.value || '').trim();
		const model: CustomFoodNutritionDto = {
			foodName,
			weight: Number(this.weight.value || 0),
			caloricValue: Number(this.customCalories.value || 0),
			carbs: Number(this.customCarbs.value || 0),
			fat: Number(this.customFat.value || 0),
			sugars: Number(this.customSugars.value || 0),
			protein: Number(this.customProtein.value || 0),
			date: this.selectedDateStr,
			mealType: this.selectedMealType,
		};

			if (!model.foodName || model.weight <= 0) {
				patchState(this.errorModal, {
					isShow: true,
					errors: ['Food name and weight are required.'],
				});
				return;
			}

			if ([model.caloricValue, model.carbs, model.fat, model.sugars, model.protein].some(value => value < 0)) {
				patchState(this.errorModal, {
					isShow: true,
					errors: ['Nutrition values cannot be negative.'],
				});
				return;
			}

		patchState(this.loader, { isShow: true });
		this.foodNutritionService
			.createCustomFoodNutritionUser(model)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					this.showAddFood = false;
					patchState(this.loader, { isShow: false });
					this.foodNutritionUsers.push(res);
					patchState(this.notice, { isShow: true, message: 'Custom food added successfully' });
					this.updateFilteredLogs();
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.errorModal, {
						isShow: true,
						errors: ['An error occurred. Please try again.'],
					});
					patchState(this.loader, { isShow: false });
				},
			});
	}

	resetSearchInput() {
		this.selectedFood = null;
		this.isCustomFoodMode = false;
		this.searchInput.setValue('');
		this.weight.setValue(0);
		this.customFoodName.setValue('');
		this.customCalories.setValue(0);
		this.customCarbs.setValue(0);
		this.customFat.setValue(0);
		this.customSugars.setValue(0);
		this.customProtein.setValue(0);
	}

	onToggleCustomFoodMode(flag: boolean) {
		this.isCustomFoodMode = flag;
		this.selectedFood = null;
		this.foods = [];
		if (flag) {
			this.customFoodName.setValue(this.searchInput.value || '');
		}
	}

	onShowDelete(flag: boolean, food: FoodNutritionUser | null) {
		this.showDelete = flag;
		this.foodSelectedToDelete = food;
	}

		onShowEdit(flag: boolean, food: FoodNutritionUser | null) {
			this.showEdit = flag;
			this.foodSelectedToEdit = food;
			if (food) {
				this.editWeight.setValue(food.weight);
				this.editCalories.setValue(food.caloricValue);
				this.editCarbs.setValue(this.getCarbs(food));
				this.editFat.setValue(food.fat);
				this.editSugars.setValue(food.sugars);
				this.editProtein.setValue(food.protein);
				this.editMealType = this.getLogMealType(food);
			}
		}

	onUpdate() {
		if (!this.foodSelectedToEdit) return;

		const item = this.foodSelectedToEdit;
			const model: UpdateFoodNutritionUserDto = {
				foodNutritionUserId: item.id,
				weight: Number(this.editWeight.value || 0),
				date: this.getLogDate(item) ?? this.selectedDateStr,
				mealType: this.editMealType,
			};

			if (model.weight <= 0) {
				patchState(this.errorModal, {
					isShow: true,
					errors: ['Weight must be greater than 0.'],
				});
				return;
			}

			if (this.isCustomLog(item)) {
				model.caloricValue = Number(this.editCalories.value || 0);
				model.carbs = Number(this.editCarbs.value || 0);
				model.fat = Number(this.editFat.value || 0);
				model.sugars = Number(this.editSugars.value || 0);
				model.protein = Number(this.editProtein.value || 0);

				if ([model.caloricValue, model.carbs, model.fat, model.sugars, model.protein].some(value => (value ?? 0) < 0)) {
					patchState(this.errorModal, {
						isShow: true,
						errors: ['Nutrition values cannot be negative.'],
					});
					return;
				}
			}

		patchState(this.loader, { isShow: true });
		this.foodNutritionService
			.updateFoodNutritionUser(model)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: res => {
					patchState(this.loader, { isShow: false });
					this.showEdit = false;

					// The backend now owns this entry's date/meal type
					this.deleteMetadata(item.id);

					this.foodNutritionUsers = this.foodNutritionUsers.map(x =>
						x.id === res.id ? res : x
					);
					this.foodSelectedToEdit = null;
					patchState(this.notice, { isShow: true, message: 'Updated successfully' });
					this.updateFilteredLogs();
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.errorModal, {
						isShow: true,
						errors: ['An error occurred. Please try again.'],
					});
					patchState(this.loader, { isShow: false });
					this.showEdit = false;
				},
			});
	}

	onDelete() {
		if (!this.foodSelectedToDelete) return;

		const idToDelete = this.foodSelectedToDelete.id;
		this.foodNutritionService
			.deleteFoodNutritionUser({
				foodNutritionUserId: idToDelete,
			})
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
				next: () => {
					patchState(this.notice, { isShow: true, message: 'Deleted successfully' });
					this.showDelete = false;

					// Remove local metadata
					this.deleteMetadata(idToDelete);

					this.foodNutritionUsers = this.foodNutritionUsers.filter(
						x => x.id !== idToDelete
					);
					this.updateFilteredLogs();
					this.foodSelectedToDelete = null;
					this.cdr.markForCheck();
				},
			});
	}

	// Budget customization methods — the backend is the source of truth so targets
	// follow the user across devices; localStorage is kept as an offline fallback.
	loadBudgets() {
		this.applyBudgets({
			calorieBudget: Number(localStorage.getItem(STORAGE_KEYS.calorieBudget) || '2000'),
			carbsBudget: Number(localStorage.getItem(STORAGE_KEYS.carbsBudget) || '250'),
			fatBudget: Number(localStorage.getItem(STORAGE_KEYS.fatBudget) || '65'),
			proteinBudget: Number(localStorage.getItem(STORAGE_KEYS.proteinBudget) || '130'),
		});

		this.foodNutritionService
			.getNutritionBudget()
			.pipe(
				catchError(() => EMPTY), // offline/error -> keep the localStorage values
				takeUntilDestroyed(this.destroyRef)
			)
			.subscribe(budget => {
				this.applyBudgets(budget);
				this.cacheBudgets(budget);
				this.updateFilteredLogs();
				this.cdr.markForCheck();
			});
	}

		saveBudgets() {
		const budget: NutritionBudget = {
			calorieBudget: this.calorieCtrl.value || 2000,
			carbsBudget: this.carbsCtrl.value || 250,
			fatBudget: this.fatCtrl.value || 65,
			proteinBudget: this.proteinCtrl.value || 130,
		};

			this.foodNutritionService
				.saveNutritionBudget(budget)
			.pipe(takeUntilDestroyed(this.destroyRef))
			.subscribe({
					next: saved => {
						this.applyBudgets(saved);
						this.cacheBudgets(saved);
						this.showSettings = false;
						patchState(this.notice, { isShow: true, message: 'Targets updated successfully!' });
					this.updateFilteredLogs();
					this.cdr.markForCheck();
				},
				error: () => {
					patchState(this.errorModal, { isShow: true, errors: ['Failed to save targets'] });
				},
			});
	}

	private applyBudgets(budget: NutritionBudget) {
		this.calorieBudget = budget.calorieBudget;
		this.carbsBudget = budget.carbsBudget;
		this.fatBudget = budget.fatBudget;
		this.proteinBudget = budget.proteinBudget;

		this.calorieCtrl.setValue(this.calorieBudget);
		this.carbsCtrl.setValue(this.carbsBudget);
		this.fatCtrl.setValue(this.fatBudget);
		this.proteinCtrl.setValue(this.proteinBudget);
	}

	private cacheBudgets(budget: NutritionBudget) {
		localStorage.setItem(STORAGE_KEYS.calorieBudget, budget.calorieBudget.toString());
		localStorage.setItem(STORAGE_KEYS.carbsBudget, budget.carbsBudget.toString());
		localStorage.setItem(STORAGE_KEYS.fatBudget, budget.fatBudget.toString());
		localStorage.setItem(STORAGE_KEYS.proteinBudget, budget.proteinBudget.toString());
	}

	// UI helper methods
	getCircleStrokeOffset(val: number, target: number): number {
		const pct = Math.min(1, Math.max(0, val / (target || 1)));
		const circumference = 2 * Math.PI * 26; // approx 163.36
		return circumference - pct * circumference;
	}

	getPercent(val: number, target: number): number {
		return Math.round(Math.min(100, Math.max(0, (val / (target || 1)) * 100)));
	}

	readonly getFoodImage = getFoodImage;

		getCarbs(item: FoodNutritionUser): number {
			return item.carbs ?? item.sugars ?? 0;
		}

		isCustomLog(item: FoodNutritionUser | null): boolean {
			return !!item && item.foodNutritionId == null;
		}

		onQuickAddRecipe(recipe: { title: string; calories: number; carbs: number; fat: number; protein: number }) {
			this.selectedMealType = 'Breakfast';
			this.isCustomFoodMode = true;
			this.showAddFood = true;
			this.selectedFood = null;
			this.foods = [];
			this.searchInput.setValue(recipe.title, { emitEvent: false });
			this.customFoodName.setValue(recipe.title);
			this.weight.setValue(1);
			this.customCalories.setValue(recipe.calories);
			this.customCarbs.setValue(recipe.carbs);
			this.customFat.setValue(recipe.fat);
			this.customSugars.setValue(0);
			this.customProtein.setValue(recipe.protein);
			this.cdr.markForCheck();
		}

	getFoodTag(item: FoodNutritionUser): string {
		if (item.protein > 15) return 'High Protein';
		if (item.fat < 3) return 'Low Fat';
		if (item.sugars > 12) return 'Sugar Alert';
		return 'Healthy Choice';
	}

	getFoodDescription(item: FoodNutritionUser): string {
		if (item.protein > 15) {
			return `Protein-packed health option containing ${item.protein.toFixed(0)}g of protein for muscle recovery.`;
		}
		if (item.fat < 3) {
			return `Heart-healthy, low-fat option with only ${item.fat.toFixed(0)}g of fat per serving.`;
		}
		if (item.sugars > 12) {
			return `Sweet indulgence with ${item.sugars.toFixed(0)}g sugar. Recommended to consume in moderation.`;
		}
		return `A balanced, nutritious choice with ${item.caloricValue.toFixed(0)} calories to keep you energized.`;
	}
}
