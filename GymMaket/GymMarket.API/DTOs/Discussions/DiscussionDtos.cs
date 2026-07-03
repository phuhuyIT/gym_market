namespace GymMarket.API.DTOs.Discussions;

public class DiscussionQuestionDto
{
    public string QuestionId { get; set; } = string.Empty;
    public string CourseId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public string? AcceptedAnswerId { get; set; }
    public int AnswerCount { get; set; }
    public bool CanAccept { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public List<DiscussionAnswerDto> Answers { get; set; } = [];
}

public class DiscussionAnswerDto
{
    public string AnswerId { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
    public string? AuthorUserId { get; set; }
    public string? AuthorEntityId { get; set; }
    public string AuthorRole { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorEmail { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsAccepted { get; set; }
    public bool CanDelete { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateDiscussionQuestionDto
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class CreateDiscussionAnswerDto
{
    public string Body { get; set; } = string.Empty;
}

public class ModerateDiscussionQuestionDto
{
    public string? Status { get; set; }
    public bool? IsPinned { get; set; }
}
