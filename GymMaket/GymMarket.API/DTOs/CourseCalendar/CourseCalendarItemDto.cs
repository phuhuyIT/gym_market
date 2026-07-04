namespace GymMarket.API.DTOs.CourseCalendar;

public class CourseCalendarItemDto
{
    public string ItemId { get; set; } = string.Empty;

    public string CourseId { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime? EndsAt { get; set; }

    public string? Status { get; set; }

    public string? Link { get; set; }
}
