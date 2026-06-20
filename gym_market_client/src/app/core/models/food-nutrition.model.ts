export interface FoodNutrition {
  id: number;
  name: string;
  caloricValue: number;
  carbs?: number | null;
  fat: number;
  sugars: number;
  protein: number;
}

export interface FoodNutritionUser {
  id: number;
  userId: string;
  foodName: string;
  weight: number;
  caloricValue: number;
  carbs?: number | null;
  fat: number;
  sugars: number;
  protein: number;
  // Null on entries logged before the backend persisted these — the
  // calculator falls back to its localStorage metadata for those.
  date?: string | null;
  mealType?: string | null;
}

// The owning user is derived from the JWT on the backend, so none of the
// request DTOs carry a userId.
export interface CaloricValueDto {
  foodNutritionId: number;
  weight: number;
  foodName: string;
  date: string;
  mealType: string;
}

export interface UpdateFoodNutritionUserDto {
  foodNutritionUserId: number;
  weight: number;
  date: string;
  mealType: string;
}

// Daily targets, stored server-side so they follow the user across devices.
export interface NutritionBudget {
  calorieBudget: number;
  carbsBudget: number;
  fatBudget: number;
  proteinBudget: number;
}
