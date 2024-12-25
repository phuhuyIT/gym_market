namespace GymMarket.API.DTOs.FoodNutritionUser
{
    public class AddFoodNutritionUser
    {
        public string UserId { get; set; } = string.Empty;
        public int FoodNutritionId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public double Weight { get; set; }

        public double Fat {  get; set; }
        public double Sugars {  get; set; }
        public double Protein {  get; set; }

    }
}
