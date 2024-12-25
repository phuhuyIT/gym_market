import { Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { UserStore } from '../../stores/user.store';
import { NoticeModalStore } from '../../stores/notice.store';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../stores/error-modal.store';
import { LoaderModalStore } from '../../stores/loader.store';
import { FoodNutritionService } from '../../pages-client/food-nutrition.service';

@Component({
	selector: 'app-food-nutrition-calculator',
	standalone: true,
	imports: [ReactiveFormsModule],
	templateUrl: './food-nutrition-calculator.component.html',
	styleUrl: './food-nutrition-calculator.component.scss',
})
export class FoodNutritionCalculatorComponent {
	showAddFood: boolean = false;
	searchInput: FormControl = new FormControl('');
	weight: FormControl = new FormControl(0);
	foods: any = [];
	selectedFood: any;
	isManualSelection = false; // Flag để kiểm soát việc tìm kiếm khi chọn item

	userStore = inject(UserStore);
	notice = inject(NoticeModalStore);
	errorModal = inject(ErrorModalStore);
	loader = inject(LoaderModalStore);

	foodNutritionUsers: any = [];

	totalCaloricValue: number = 0;
	totalFat: number = 0;
	totalSugar: number = 0;
	totalProtein: number = 0;

	// delete
	showDelete: boolean = false;
	foodSelectedToDelete: any;

	constructor(private foodNutritionService: FoodNutritionService) {}

	ngOnInit() {
		this.getFoodNutritionUser();

        
		this.searchInput.valueChanges
			.pipe(
				debounceTime(500),
				distinctUntilChanged() // Đặt thời gian debounce là 500ms
			)
			.subscribe(value => {
                patchState(this.loader, {isShow: true})
				if (this.isManualSelection) {
					this.isManualSelection = false; // Bỏ qua nếu là thay đổi từ chọn item
					this.foods = [];patchState(this.loader, {isShow: false})
					return;
				}
				this.foodNutritionService.search(value).subscribe({
					next: (res: any) => {
						// console.log(res);
						this.foods = res;
                        patchState(this.loader, {isShow: false})
					},
					error: err => {
						console.log(err);
                        patchState(this.loader, {isShow: false})
					},
				});
			});
	}

	private getFoodNutritionUser() {
		if (this.userStore.id()) {
			this.foodNutritionService.getFoodNutritionUser(this.userStore.id()).subscribe({
				next: (res: any) => {
					this.foodNutritionUsers = res;
					// console.log(res);
					this.calStatistics();
				},
				error: err => {
					console.log(err);
				},
			});
		}
	}

	calStatistics() {
		this.totalCaloricValue = this.foodNutritionUsers.reduce(
			(sum: any, food: any) => sum + food.caloricValue,
			0
		);
		this.totalFat = this.foodNutritionUsers.reduce((sum: any, food: any) => sum + food.fat, 0);
		this.totalSugar = this.foodNutritionUsers.reduce(
			(sum: any, food: any) => sum + food.sugars,
			0
		);
		this.totalProtein = this.foodNutritionUsers.reduce(
			(sum: any, food: any) => sum + food.protein,
			0
		);
	}

	onSelectFood(food: any) {
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
		if (!this.selectedFood) {
		}

		const model = {
			userId: this.userStore.id(),
			foodNutritionId: this.selectedFood.id,
			foodName: this.selectedFood.name,
			weight: this.weight.value,
		};
		patchState(this.loader, { isShow: true });
		this.foodNutritionService.calCaloricValue(model).subscribe({
			next: (res: any) => {
				console.log(res);
				this.showAddFood = false;
				patchState(this.loader, { isShow: false });
				this.foodNutritionUsers.push(res);
				patchState(this.notice, { isShow: true, message: 'Successfully' });
                this.calStatistics();
			},
			error: (err: any) => {
				console.log(err);
				patchState(this.errorModal, {
					isShow: true,
					errors: ['Có lỗi xảy ra. Vui lòng thử lại.'],
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

	onShowDelete(flag: boolean, food: any) {
		this.showDelete = flag;

		if (flag === true) {
			this.foodSelectedToDelete = food;
		} else {
			this.foodSelectedToDelete = null;
		}
	}

	onDelete() {
		this.foodNutritionService
			.deleteFoodNutritionUser({
				UserId: this.userStore.id(),
				foodNutritionUserId: this.foodSelectedToDelete.id,
			})
			.subscribe({
				next: (res: any) => {
					patchState(this.notice, { isShow: true, message: 'Successfully' });
					this.showDelete = false;

					const index = this.foodNutritionUsers.findIndex(
						(x: any) => x.id === this.foodSelectedToDelete.id
					);
					if (index !== -1) {
						this.foodNutritionUsers.splice(index, 1);
					}
					this.calStatistics();
					this.foodSelectedToDelete = null;
				},
				error: err => {
					console.log(err);
				},
			});
	}
}
