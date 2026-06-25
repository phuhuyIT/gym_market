export const ACTIVITY_DAYS = [
	{ day: 'S', hours: 1, label: '1hr' },
	{ day: 'M', hours: 1.5, label: '1hr 30m' },
	{ day: 'T', hours: 2.5, label: '2hr 30m' },
	{ day: 'W', hours: 2, label: '2hr' },
	{ day: 'Th', hours: 3.5, label: '3hr 30min' },
	{ day: 'F', hours: 3, label: '3hr' },
	{ day: 'S', hours: 0.8, label: '48m' }
];

export const RECIPES_LIST = [
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

const FOOD_IMAGES = {
	meat:      'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&auto=format&fit=crop&q=60',
	salad:     'https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=400&auto=format&fit=crop&q=60',
	breakfast: 'https://images.unsplash.com/photo-1525351484163-7529414344d8?w=400&auto=format&fit=crop&q=60',
	fruit:     'https://images.unsplash.com/photo-1519985176271-adb1088fa94c?w=400&auto=format&fit=crop&q=60',
	shake:     'https://images.unsplash.com/photo-1553530666-ba11a7da3888?w=400&auto=format&fit=crop&q=60',
	default:   'https://images.unsplash.com/photo-1498837167922-ddd27525d352?w=400&auto=format&fit=crop&q=60',
} as const;

export function getFoodImage(foodName: string): string {
	const name = foodName.toLowerCase();
	if (name.includes('chicken') || name.includes('poultry') || name.includes('meat') || name.includes('pork') || name.includes('beef')) return FOOD_IMAGES.meat;
	if (name.includes('salad') || name.includes('lettuce') || name.includes('vegetable') || name.includes('green') || name.includes('tomato') || name.includes('cabbage')) return FOOD_IMAGES.salad;
	if (name.includes('egg') || name.includes('toast') || name.includes('bread') || name.includes('pancake') || name.includes('oat') || name.includes('cereal') || name.includes('butter')) return FOOD_IMAGES.breakfast;
	if (name.includes('apple') || name.includes('banana') || name.includes('fruit') || name.includes('berry') || name.includes('orange') || name.includes('avocado') || name.includes('peach')) return FOOD_IMAGES.fruit;
	if (name.includes('shake') || name.includes('smoothie') || name.includes('protein') || name.includes('milk') || name.includes('yogurt') || name.includes('juice') || name.includes('drink')) return FOOD_IMAGES.shake;
	return FOOD_IMAGES.default;
}
