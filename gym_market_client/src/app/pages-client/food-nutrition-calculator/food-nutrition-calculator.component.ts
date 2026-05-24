import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit , ChangeDetectionStrategy } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
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
import { SEARCH_DEBOUNCE_MS } from '../../utilities/defaults.const';


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

	// Mock healthy recipes
	recipesList = [
		{
			title: 'High Protein Chicken Salad',
			desc: 'A fresh, high-protein salad featuring grilled chicken breast, spinach, cucumber, and a light olive oil vinaigrette.',
			calories: 450,
			protein: 42,
			carbs: 12,
			fat: 14,
			image: 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&auto=format&fit=crop&q=60'
		},
		{
			title: 'Avocado Toast & Eggs',
			desc: 'Crisp sourdough toast topped with creamy mashed avocado, two poached eggs, and red pepper flakes.',
			calories: 380,
			protein: 18,
			carbs: 24,
			fat: 22,
			image: 'https://images.unsplash.com/photo-1525351484163-7529414344d8?w=400&auto=format&fit=crop&q=60'
		},
		{
			title: 'Mixed Berry Protein Shake',
			desc: 'A delicious blend of organic berries, Greek yogurt, whey protein isolate, and unsweetened almond milk.',
			calories: 290,
			protein: 26,
			carbs: 30,
			fat: 4,
			image: 'https://images.unsplash.com/photo-1553530666-ba11a7da3888?w=400&auto=format&fit=crop&q=60'
		},
		{
			title: 'Grilled Salmon & Quinoa Bowl',
			desc: 'Wild-caught salmon served alongside a fluffy quinoa salad, steamed asparagus, and lemon herb drizzle.',
			calories: 520,
			protein: 38,
			carbs: 35,
			fat: 18,
			image: 'https://images.unsplash.com/photo-1498837167922-ddd27525d352?w=400&auto=format&fit=crop&q=60'
		}
	];

	constructor(private foodNutritionService: FoodNutritionService) {}

	ngOnInit() {
		this.loadBudgets();
		
		const today = new Date();
		this.selectedDateStr = this.formatDate(today);
		this.generateWeekForDate(today);

		this.getFoodNutritionUser();

		this.searchInput.valueChanges
			.pipe(debounceTime(SEARCH_DEBOUNCE_MS), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
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
						this.cdr.markForCheck();
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
		const data = localStorage.getItem('gym_market_nutrition_metadata');
		return data ? JSON.parse(data) : {};
	}

	setMetadata(id: number, date: string, mealType: string) {
		const metadata = this.getMetadata();
		metadata[id] = { date, mealType };
		localStorage.setItem('gym_market_nutrition_metadata', JSON.stringify(metadata));
	}

	deleteMetadata(id: number) {
		const metadata = this.getMetadata();
		delete metadata[id];
		localStorage.setItem('gym_market_nutrition_metadata', JSON.stringify(metadata));
	}

	updateFilteredLogs() {
		const metadata = this.getMetadata();
		const meals = ['Breakfast', 'Lunch', 'Dinner', 'Snacks'];
		let updated = false;

		// Distribute unmapped logs dynamically
		this.foodNutritionUsers.forEach((item, index) => {
			if (!metadata[item.id]) {
				const mealType = meals[index % meals.length];
				metadata[item.id] = {
					date: this.selectedDateStr,
					mealType: mealType
				};
				updated = true;
			}
		});

		if (updated) {
			localStorage.setItem('gym_market_nutrition_metadata', JSON.stringify(metadata));
		}

		// Filter logs for selected date
		this.filteredLogs = this.foodNutritionUsers.filter(item => {
			const meta = metadata[item.id];
			return meta && meta.date === this.selectedDateStr;
		});

		// Check dot indicators for week days
		this.weekDays.forEach(day => {
			day.hasLogs = this.foodNutritionUsers.some(item => {
				const meta = metadata[item.id];
				return meta && meta.date === day.fullDate;
			});
		});

		// Distribute logs by meal
		this.breakfastLogs = [];
		this.lunchLogs = [];
		this.dinnerLogs = [];
		this.snacksLogs = [];

		this.filteredLogs.forEach(item => {
			const mealType = metadata[item.id]?.mealType || 'Breakfast';
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
					
					// Save local metadata
					this.setMetadata(res.id, this.selectedDateStr, this.selectedMealType);

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

	onDelete() {
		if (!this.foodSelectedToDelete) return;

		const idToDelete = this.foodSelectedToDelete.id;
		this.foodNutritionService
			.deleteFoodNutritionUser({
				userId: this.userStore.id() ?? '',
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
		this.calorieBudget = Number(localStorage.getItem('gym_market_calorie_budget') || '2000');
		this.carbsBudget = Number(localStorage.getItem('gym_market_carbs_budget') || '250');
		this.fatBudget = Number(localStorage.getItem('gym_market_fat_budget') || '65');
		this.proteinBudget = Number(localStorage.getItem('gym_market_protein_budget') || '130');

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

		localStorage.setItem('gym_market_calorie_budget', this.calorieBudget.toString());
		localStorage.setItem('gym_market_carbs_budget', this.carbsBudget.toString());
		localStorage.setItem('gym_market_fat_budget', this.fatBudget.toString());
		localStorage.setItem('gym_market_protein_budget', this.proteinBudget.toString());

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

	getFoodImage(foodName: string): string {
		const name = foodName.toLowerCase();
		if (name.includes('chicken') || name.includes('poultry') || name.includes('meat') || name.includes('pork') || name.includes('beef')) {
			return 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&auto=format&fit=crop&q=60';
		}
		if (name.includes('salad') || name.includes('lettuce') || name.includes('vegetable') || name.includes('green') || name.includes('tomato') || name.includes('cabbage')) {
			return 'https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=400&auto=format&fit=crop&q=60';
		}
		if (name.includes('egg') || name.includes('toast') || name.includes('bread') || name.includes('pancake') || name.includes('oat') || name.includes('cereal') || name.includes('butter')) {
			return 'https://images.unsplash.com/photo-1525351484163-7529414344d8?w=400&auto=format&fit=crop&q=60';
		}
		if (name.includes('apple') || name.includes('banana') || name.includes('fruit') || name.includes('berry') || name.includes('orange') || name.includes('avocado') || name.includes('peach')) {
			return 'https://images.unsplash.com/photo-1519985176271-adb1088fa94c?w=400&auto=format&fit=crop&q=60';
		}
		if (name.includes('shake') || name.includes('smoothie') || name.includes('protein') || name.includes('milk') || name.includes('yogurt') || name.includes('juice') || name.includes('drink')) {
			return 'https://images.unsplash.com/photo-1553530666-ba11a7da3888?w=400&auto=format&fit=crop&q=60';
		}
		return 'https://images.unsplash.com/photo-1498837167922-ddd27525d352?w=400&auto=format&fit=crop&q=60';
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
			return `Sweet indulgence with ${item.sugars.toFixed(0)}g carbs. Recommended to consume in moderation.`;
		}
		return `A balanced, nutritious choice with ${item.caloricValue.toFixed(0)} calories to keep you energized.`;
	}
}
