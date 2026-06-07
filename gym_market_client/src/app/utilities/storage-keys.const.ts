/**
 * Centralised localStorage keys.
 *
 * Note: the string values are intentionally inconsistent (`gym-token`,
 * `gym_market_*`, `gym_bookmarked_*`) because they match data already
 * persisted in users' browsers — changing a value would orphan that data.
 * Reference these constants instead of duplicating the literals.
 */
export const STORAGE_KEYS = {
	token: 'gym-token',
	refreshToken: 'gym-refresh-token',
	theme: 'gymmarket-theme',
	bookmarkedTrainers: 'gym_bookmarked_trainers',
	bookmarkedCourses: 'gym_bookmarked_courses',
	nutritionMetadata: 'gym_market_nutrition_metadata',
	calorieBudget: 'gym_market_calorie_budget',
	carbsBudget: 'gym_market_carbs_budget',
	fatBudget: 'gym_market_fat_budget',
	proteinBudget: 'gym_market_protein_budget',
} as const;
