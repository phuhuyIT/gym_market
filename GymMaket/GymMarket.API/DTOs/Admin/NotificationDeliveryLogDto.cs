namespace GymMarket.API.DTOs.Admin;

public class NotificationDeliveryLogDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string? RecipientEmail { get; set; }

    public string? RecipientName { get; set; }

    public string Type { get; set; } = string.Empty;

    public string TypeLabel { get; set; } = string.Empty;

    public string Channel { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string? Content { get; set; }

    public string? Link { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; }
}
