import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
	CaloricValueDto,
	FoodNutrition,
	FoodNutritionUser,
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

	getFoodNutritionUser(userId: string | null): Observable<FoodNutritionUser[]> {
		return this.http.get<FoodNutritionUser[]>(
			`${environment.baseApi}/FoodNutrition/get-nutrition-user/${userId}`
		);
	}

	calCaloricValue(model: CaloricValueDto): Observable<FoodNutritionUser> {
		return this.http.post<FoodNutritionUser>(
			`${environment.baseApi}/FoodNutrition/cal-caloric-value`,
			model
		);
	}

	deleteFoodNutritionUser(model: { userId: string; foodNutritionUserId: number }): Observable<void> {
		return this.http.post<void>(
			`${environment.baseApi}/FoodNutrition/delete-foodnutrition-user`,
			model
		);
	}
}
