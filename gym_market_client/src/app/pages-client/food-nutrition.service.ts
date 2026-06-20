import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
	CaloricValueDto,
	CustomFoodNutritionDto,
	FoodNutrition,
	FoodNutritionUpsertDto,
	FoodNutritionUser,
	NutritionBudget,
	NutritionSummary,
	UpdateFoodNutritionUserDto,
} from '../core/models/food-nutrition.model';

@Injectable({
	providedIn: 'root',
})
export class FoodNutritionService {
	constructor(private http: HttpClient) {}

	search(search = '', page = 1, pageSize = 20): Observable<FoodNutrition[]> {
		const params = new HttpParams()
			.set('search', search.trim())
			.set('page', page)
			.set('pageSize', pageSize);

		return this.http.get<FoodNutrition[]>(
			`${environment.baseApi}/FoodNutrition/search-nutrition`,
			{ params }
		);
	}

	getFoodNutritionUser(date?: string, page?: number, pageSize?: number): Observable<FoodNutritionUser[]> {
		let params = new HttpParams();
		if (date) params = params.set('date', date);
		if (page) params = params.set('page', page);
		if (pageSize) params = params.set('pageSize', pageSize);

		return this.http.get<FoodNutritionUser[]>(
			`${environment.baseApi}/FoodNutrition/get-nutrition-user`,
			{ params }
		);
	}

	getFoodNutritionUserRange(from: string, to: string): Observable<FoodNutritionUser[]> {
		const params = new HttpParams().set('from', from).set('to', to);
		return this.http.get<FoodNutritionUser[]>(
			`${environment.baseApi}/FoodNutrition/get-nutrition-user-range`,
			{ params }
		);
	}

	getNutritionSummary(from: string, to: string): Observable<NutritionSummary[]> {
		const params = new HttpParams().set('from', from).set('to', to);
		return this.http.get<NutritionSummary[]>(
			`${environment.baseApi}/FoodNutrition/nutrition-summary`,
			{ params }
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

	createFoodNutrition(model: FoodNutritionUpsertDto): Observable<FoodNutrition> {
		return this.http.post<FoodNutrition>(
			`${environment.baseApi}/FoodNutrition/create-nutrition`,
			model
		);
	}

	updateFoodNutrition(id: number, model: FoodNutritionUpsertDto): Observable<FoodNutrition> {
		return this.http.put<FoodNutrition>(
			`${environment.baseApi}/FoodNutrition/update-nutrition/${id}`,
			model
		);
	}

	deleteFoodNutrition(id: number): Observable<void> {
		return this.http.delete<void>(
			`${environment.baseApi}/FoodNutrition/delete-nutrition/${id}`
		);
	}
}
