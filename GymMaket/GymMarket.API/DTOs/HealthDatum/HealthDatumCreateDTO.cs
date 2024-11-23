using GymMarket.API.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.HealthDatum
{
    public class HealthDatumCreateDTO
    {
        [Required(ErrorMessage = "HealthDataId is required.")]
        public string HealthDataId { get; set; } = null!;

        [Required(ErrorMessage = "StudentId is required.")]
        [StringLength(50, ErrorMessage = "StudentId cannot exceed 50 characters.")]
        public string? StudentId { get; set; }

        [StringLength(50, ErrorMessage = "CourseId cannot exceed 50 characters.")]
        public string? CourseId { get; set; }

        [StringLength(255, ErrorMessage = "BodyImage URL cannot exceed 255 characters.")]
        [Url(ErrorMessage = "BodyImage must be a valid URL.")]
        public string? BodyImage { get; set; }

        [StringLength(1000, ErrorMessage = "ProgressNotes cannot exceed 1000 characters.")]
        public string? ProgressNotes { get; set; }

        [Required(ErrorMessage = "CreatedAt is required.")]
        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required(ErrorMessage = "At least one HealthIndicator is required.")]
        public virtual ICollection<HealthIndicator> HealthIndicators { get; set; } = new List<HealthIndicator>();
    }
}
