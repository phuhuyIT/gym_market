namespace GymMarket.API.DTOs.Admin;

public class AdminNotificationTemplateDto
{
    public string Type { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public string SubjectTemplate { get; set; } = string.Empty;

    public string BodyTemplate { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public IReadOnlyList<string> Variables { get; set; } = [];
}
