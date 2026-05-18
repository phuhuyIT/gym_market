namespace GymMarket.API.DTOs.LectureMaterial
{
    public class GetLectureMaterialDto
    {
        public string MaterialId { get; set; } = null!;
        public string? LectureId { get; set; }
        public string? MaterialType { get; set; }
        public string? MaterialContent { get; set; }
    }
}
