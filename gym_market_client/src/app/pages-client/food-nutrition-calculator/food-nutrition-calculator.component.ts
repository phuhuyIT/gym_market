import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { debounceTime, distinctUntilChanged, switchMap, catchError, EMPTY } from 'rxjs';
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

	userStore = inject(UserStore);
	notice = inject(NoticeModalStore);
	errorModal = inject(ErrorModalStore);
	loader = inject(LoaderModalStore);
	private destroyRef = inject(DestroyRef);
	private cdr = inject(ChangeDetectorRef);

	foodNutritionUsers: FoodNutritionUser[] = [];

	totalCaloricValue: number = 0;
	totalFat: number = 0;
	totalSugar: number = 0;
	totalProtein: number = 0;

	showDelete: boolean = false;
	foodSelectedToDelete: FoodNutritionUser | null = null;
	showSettings: boolean = false;

	showEdit: boolean = false;
	foodSelectedToEdit: FoodNutritionUser | null = null;
	editWeight: FormControl = new FormControl(0);
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

	readonly recipesList = RECIPES_LIST;

	constructor(private foodNutritionService: FoodNutritionService) {}

	ngOnInit() {
		this.loadBudgets();
		
		const today = new Date();
		this.selectedDateStr = this.formatDate(today);
		this.generateWeekForDate(today);

		this.getFoodNutritionUser();

		this.searchInput.valueChanges
			.pipe(
				debounceTime(SEARCH_DEBOUNCE_MS),
				distinctUntilChanged(),
				switchMap(value => {
					patchState(this.loader, { isShow: true });
					if (this.isManualSelection) {
						this.isManualSelection = false;
						this.foods = [];
						patchState(this.loader, { isShow: false });
						return EMPTY;
					}
					return this.foodNutritionService.search(value).pipe(
						catchError(() => {
							patchState(this.loader, { isShow: false });
							return EMPTY;
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
			day.hasLogs = this.foodNutritionUsers.some(
				item => this.getLogDate(item, metadata) === day.fullDate
			);
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
			this.updateFilteredLogs();
			this.cdr.markForCheck();
		}
	}

	calStatistics() {
		this.totalCaloricValue = this.filteredLogs.reduce(
			(sum, food) => sum + (food.caloricValue ?? 0),
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
		if (!this.selectedFood) return;

		const model: CaloricValueDto = {
			foodNutritionId: this.selectedFood.id,
			weight: this.weight.value,
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

	resetSearchInput() {
		this.selectedFood = null;
		this.searchInput.setValue('');
		this.weight.setValue(0);
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
			this.editMealType = this.getLogMealType(food);
		}
	}

	onUpdate() {
		if (!this.foodSelectedToEdit) return;

		const item = this.foodSelectedToEdit;
		const model: UpdateFoodNutritionUserDto = {
			foodNutritionUserId: item.id,
			weight: this.editWeight.value,
			date: this.getLogDate(item) ?? this.selectedDateStr,
			mealType: this.editMealType,
		};

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

	// Budget customization methods
	loadBudgets() {
		this.calorieBudget = Number(localStorage.getItem(STORAGE_KEYS.calorieBudget) || '2000');
		this.carbsBudget = Number(localStorage.getItem(STORAGE_KEYS.carbsBudget) || '250');
		this.fatBudget = Number(localStorage.getItem(STORAGE_KEYS.fatBudget) || '65');
		this.proteinBudget = Number(localStorage.getItem(STORAGE_KEYS.proteinBudget) || '130');

		this.calorieCtrl.setValue(this.calorieBudget);
		this.carbsCtrl.setValue(this.carbsBudget);
		this.fatCtrl.setValue(this.fatBudget);
		this.proteinCtrl.setValue(this.proteinBudget);
	}

	saveBudgets() {
		this.calorieBudget = this.calorieCtrl.value || 2000;
		this.carbsBudget = this.carbsCtrl.value || 250;
		this.fatBudget = this.fatCtrl.value || 65;
		this.proteinBudget = this.proteinCtrl.value || 130;

		localStorage.setItem(STORAGE_KEYS.calorieBudget, this.calorieBudget.toString());
		localStorage.setItem(STORAGE_KEYS.carbsBudget, this.carbsBudget.toString());
		localStorage.setItem(STORAGE_KEYS.fatBudget, this.fatBudget.toString());
		localStorage.setItem(STORAGE_KEYS.proteinBudget, this.proteinBudget.toString());

		patchState(this.notice, { isShow: true, message: 'Targets updated successfully!' });
		this.updateFilteredLogs();
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
			return `Sweet indulgence with ${item.sugars.toFixed(0)}g carbs. Recommended to consume in moderation.`;
		}
		return `A balanced, nutritious choice with ${item.caloricValue.toFixed(0)} calories to keep you energized.`;
	}
}
