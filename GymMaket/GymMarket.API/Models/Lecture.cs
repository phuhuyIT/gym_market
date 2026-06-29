using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class Lecture
{
    public string LectureId { get; set; } = null!;

    public string? CourseId { get; set; }

    public string? ModuleId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? ActivityType { get; set; }

    public int? Order { get; set; }

    public int? Duration { get; set; }

    public string? PrerequisiteLectureId { get; set; }

    public int? UnlockAfterDays { get; set; }

    public DateTime? AvailableFrom { get; set; }

    public DateTime? AvailableUntil { get; set; }

    public bool IsPreview { get; set; }

    public bool IsPublished { get; set; } = true;

    public virtual Course? Course { get; set; }

    public virtual CourseModule? Module { get; set; }

    public virtual Lecture? PrerequisiteLecture { get; set; }

    public virtual ICollection<Lecture> DependentLectures { get; set; } = new List<Lecture>();

    public virtual ICollection<LectureMaterial> LectureMaterials { get; set; } = new List<LectureMaterial>();

    public virtual ICollection<LectureProgress> LectureProgresses { get; set; } = new List<LectureProgress>();
}
