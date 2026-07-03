using System;

namespace GymMarket.API.Models;

public class CourseDiscussionAnswer
{
    public string AnswerId { get; set; } = null!;

    public string QuestionId { get; set; } = null!;

    public string? AuthorUserId { get; set; }

    public string? AuthorEntityId { get; set; }

    public string AuthorRole { get; set; } = DiscussionAuthorRole.Student;

    public string AuthorName { get; set; } = null!;

    public string? AuthorEmail { get; set; }

    public string Body { get; set; } = null!;

    public bool IsAccepted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual CourseDiscussionQuestion Question { get; set; } = null!;
}
