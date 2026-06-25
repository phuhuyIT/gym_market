using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class Lecture
{
    public string LectureId { get; set; } = null!;

    public string? CourseId { get; set; }

    public string? Title { get; set; }

    public int? Order { get; set; }

    public int? Duration { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<LectureMaterial> LectureMaterials { get; set; } = new List<LectureMaterial>();

    public virtual ICollection<LectureProgress> LectureProgresses { get; set; } = new List<LectureProgress>();
}
