using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymMarket.API.Models
{
    // A user's daily nutrition targets (one row per user). Values mirror the
    // calculator UI: calories in kcal, the macros in grams.
    [Table("NutritionBudgets")]
    public class NutritionBudget
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public double CalorieBudget { get; set; }
        public double CarbsBudget { get; set; }
        public double FatBudget { get; set; }
        public double ProteinBudget { get; set; }
    }
}
