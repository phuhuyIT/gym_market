using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.FoodNutrition
{
    public class NutritionBudgetDto
    {
        [Range(1, double.MaxValue, ErrorMessage = "CalorieBudget must be greater than 0.")]
        public double CalorieBudget { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "CarbsBudget must be greater than 0.")]
        public double CarbsBudget { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "FatBudget must be greater than 0.")]
        public double FatBudget { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "ProteinBudget must be greater than 0.")]
        public double ProteinBudget { get; set; }
    }
}
