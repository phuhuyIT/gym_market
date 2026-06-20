namespace GymMarket.API.DTOs.FoodNutrition
{
    public class NutritionSummaryDto
    {
        public DateOnly Date { get; set; }
        public double CaloricValue { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public double Sugars { get; set; }
        public double Protein { get; set; }
        public int EntryCount { get; set; }
    }
}
