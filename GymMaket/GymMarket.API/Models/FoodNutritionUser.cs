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

        public double Fat { get; set; }
        public double Sugars { get; set; }
        public double Protein { get; set; }
    }
}
