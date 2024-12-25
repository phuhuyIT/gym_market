import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';

@Injectable({
	providedIn: 'root',
})
export class FoodNutritionService {
	constructor(private http: HttpClient) {}

	search(search: string) {
		return this.http.get(
			`${environment.baseApi}/FoodNutrition/search-nutrition?search=${search}`
		);
	}

    getFoodNutritionUser(userId: string|null) {
		return this.http.get(
			`${environment.baseApi}/FoodNutrition/get-nutrition-user/${userId}`
		);
	}

	calCaloricValue(model: any) {
		return this.http.post(`${environment.baseApi}/FoodNutrition/cal-caloric-value`, model);
	}

    deleteFoodNutritionUser(model: any) {
        return this.http.post(`${environment.baseApi}/FoodNutrition/delete-foodnutrition-user`, model);
    }
}
