namespace GymMarket.API.Models;

public class CourseStudyGroup
{
    public string StudyGroupId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public int ConversationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Kind { get; set; } = CourseStudyGroupKind.StudyGroup;

    public bool IsDefaultCohort { get; set; }

    public bool IsActive { get; set; } = true;

    public string? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Conversation Conversation { get; set; } = null!;
}

public static class CourseStudyGroupKind
{
    public const string Cohort = "Cohort";
    public const string StudyGroup = "StudyGroup";

    public static readonly string[] All = [Cohort, StudyGroup];

    public static string Normalize(string? value)
    {
        if (string.Equals(value, Cohort, StringComparison.OrdinalIgnoreCase))
            return Cohort;

        return StudyGroup;
    }
}
