import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
	CaloricValueDto,
	CustomFoodNutritionDto,
	FoodNutrition,
	FoodNutritionUser,
	NutritionBudget,
	UpdateFoodNutritionUserDto,
} from '../core/models/food-nutrition.model';

@Injectable({
	providedIn: 'root',
})
export class FoodNutritionService {
	constructor(private http: HttpClient) {}

	search(search: string): Observable<FoodNutrition[]> {
		return this.http.get<FoodNutrition[]>(
			`${environment.baseApi}/FoodNutrition/search-nutrition?search=${search}`
		);
	}

	getFoodNutritionUser(): Observable<FoodNutritionUser[]> {
		return this.http.get<FoodNutritionUser[]>(
			`${environment.baseApi}/FoodNutrition/get-nutrition-user`
		);
	}

	calCaloricValue(model: CaloricValueDto): Observable<FoodNutritionUser> {
		return this.http.post<FoodNutritionUser>(
			`${environment.baseApi}/FoodNutrition/cal-caloric-value`,
			model
		);
	}

	createCustomFoodNutritionUser(model: CustomFoodNutritionDto): Observable<FoodNutritionUser> {
		return this.http.post<FoodNutritionUser>(
			`${environment.baseApi}/FoodNutrition/custom-foodnutrition-user`,
			model
		);
	}

	updateFoodNutritionUser(model: UpdateFoodNutritionUserDto): Observable<FoodNutritionUser> {
		return this.http.put<FoodNutritionUser>(
			`${environment.baseApi}/FoodNutrition/update-foodnutrition-user`,
			model
		);
	}

	deleteFoodNutritionUser(model: { foodNutritionUserId: number }): Observable<void> {
		return this.http.delete<void>(
			`${environment.baseApi}/FoodNutrition/delete-foodnutrition-user`,
			{ body: model }
		);
	}

	getNutritionBudget(): Observable<NutritionBudget> {
		return this.http.get<NutritionBudget>(
			`${environment.baseApi}/FoodNutrition/nutrition-budget`
		);
	}

	saveNutritionBudget(model: NutritionBudget): Observable<NutritionBudget> {
		return this.http.put<NutritionBudget>(
			`${environment.baseApi}/FoodNutrition/nutrition-budget`,
			model
		);
	}
}
