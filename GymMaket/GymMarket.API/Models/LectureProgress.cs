using System;

namespace GymMarket.API.Models;

public partial class LectureProgress
{
    public string LectureProgressId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string LectureId { get; set; } = null!;

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Lecture? Lecture { get; set; }

    public virtual Student? Student { get; set; }
}
