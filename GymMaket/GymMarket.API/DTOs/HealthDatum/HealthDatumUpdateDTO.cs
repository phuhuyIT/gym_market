using GymMarket.API.Models;
using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.HealthDatum
{
    public class HealthDatumUpdateDTO
    {
        [Url(ErrorMessage = "BodyImage must be a valid URL.")]
        public string? BodyImage { get; set; }

        [StringLength(500, ErrorMessage = "ProgressNotes cannot exceed 500 characters.")]
        public string? ProgressNotes { get; set; }
        [Required(ErrorMessage = "UpdatedAt is required.")]
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<HealthIndicator> HealthIndicators { get; set; } = new List<HealthIndicator>();
    }
}
