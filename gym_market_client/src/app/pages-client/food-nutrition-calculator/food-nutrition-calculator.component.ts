import { Component, inject } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { FoodNutritionService } from '../food-nutrition.service';
import { UserStore } from '../../stores/user.store';
import { NoticeModalStore } from '../../stores/notice.store';
import { patchState } from '@ngrx/signals';
import { ErrorModalStore } from '../../stores/error-modal.store';

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

	foodNutritionUsers: any = [];

	constructor(private foodNutritionService: FoodNutritionService) {}

	ngOnInit() {
		this.getFoodNutritionUser();
		this.searchInput.valueChanges
			.pipe(
				debounceTime(500),
				distinctUntilChanged() // Đặt thời gian debounce là 500ms
			)
			.subscribe(value => {
				if (this.isManualSelection) {
					this.isManualSelection = false; // Bỏ qua nếu là thay đổi từ chọn item
					this.foods = [];
					return;
				}
				this.foodNutritionService.search(value).subscribe({
					next: (res: any) => {
						// console.log(res);
						this.foods = res;
					},
					error: err => {
						console.log(err);
					},
				});
			});
	}

	private getFoodNutritionUser() {
		if (this.userStore.id()) {
			this.foodNutritionService.getFoodNutritionUser(this.userStore.id()).subscribe({
				next: (res: any) => {
					this.foodNutritionUsers = res;
					console.log(res);
				},
				error: err => {
					console.log(err);
				},
			});
		}
	}

	onSelectFood(food: any) {
		this.isManualSelection = true;
		this.selectedFood = food;
		this.searchInput.setValue(food.name);
		this.foods = [];
	}

	onShowAddFoodNutrition() {
		this.showAddFood = true;
	}

	onCal() {
		const model = {
			userId: this.userStore.id(),
			foodNutritionId: this.selectedFood.id,
			foodName: this.selectedFood.name,
			weight: this.weight.value,
		};
		this.foodNutritionService.calCaloricValue(model).subscribe({
			next: (res: any) => {
				console.log(res);
				this.showAddFood = false;
				this.foodNutritionUsers.push(res);
				patchState(this.notice, { isShow: true, message: 'Thêm thành công' });
			},
			error: (err: any) => {
				console.log(err);
				patchState(this.errorModal, {
					isShow: true,
					errors: ['Có lỗi xảy ra. Vui lòng thử lại.'],
				});
				this.showAddFood = false;
			},
		});
	}
}
