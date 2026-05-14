export interface FoodNutrition {
  id: number;
  name: string;
  caloricValue: number;
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
  fat: number;
  sugars: number;
  protein: number;
  date?: string;
}

export interface CaloricValueDto {
  userId: string | null;
  foodNutritionId: number;
  weight: number;
  foodName: string;
}
