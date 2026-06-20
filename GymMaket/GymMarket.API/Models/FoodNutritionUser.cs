using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymMarket.API.Models
{
    [Table("FoodNutritionUsers")]
    public class FoodNutritionUser
    {
        [Key]
        public int Id { get; set; }

        public string FoodName { get; set; } = string.Empty;
        public double Weight { get; set; }
        public string UserId { get; set; } = string.Empty;

        public double CaloricValue { get; set; }

        public double Carbs { get; set; }
        public double Fat { get; set; }
        public double Sugars { get; set; }
        public double Protein { get; set; }

        // Nullable so rows logged before these columns existed stay distinguishable
        // (the client falls back to its localStorage metadata for them).
        public DateOnly? Date { get; set; }
        public string? MealType { get; set; }

        // Source food in the master database; null for rows logged before the FK
        // existed or whose master food was deleted (the log keeps its snapshot).
        public int? FoodNutritionId { get; set; }
    }
}
