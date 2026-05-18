using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.LectureMaterial
{
    public class LectureMaterialUpdateDTO
    {
        [Required(ErrorMessage = "MaterialId is required.")]
        public string MaterialId { get; set; } = null!;

        public string? LectureId { get; set; }

        [StringLength(50)]
        public string? MaterialType { get; set; }

        public string? MaterialContent { get; set; }
    }
}
