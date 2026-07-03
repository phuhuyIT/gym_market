using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public class CourseDiscussionQuestion
{
    public string QuestionId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public string Status { get; set; } = DiscussionQuestionStatus.Open;

    public bool IsPinned { get; set; }

    public string? AcceptedAnswerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime LastActivityAt { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual ICollection<CourseDiscussionAnswer> Answers { get; set; } = new List<CourseDiscussionAnswer>();
}
