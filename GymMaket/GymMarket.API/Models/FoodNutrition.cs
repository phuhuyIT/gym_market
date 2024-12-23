using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymMarket.API.Models
{
    [Table("FoodNutritions")]
    public class FoodNutrition
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long CaloricValue { get; set; }
    }
}
