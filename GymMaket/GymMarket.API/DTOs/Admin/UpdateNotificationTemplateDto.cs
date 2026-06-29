using System.ComponentModel.DataAnnotations;

namespace GymMarket.API.DTOs.Admin;

public class UpdateNotificationTemplateDto
{
    [Required]
    [MaxLength(250)]
    public string SubjectTemplate { get; set; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string BodyTemplate { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
