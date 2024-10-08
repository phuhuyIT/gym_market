using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class CourseRating
{
    public string RatingId { get; set; } = null!;

    public string? CourseId { get; set; }

    public string? StudentId { get; set; }

    public decimal? RatingValue { get; set; }

    public string? ReviewComment { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Student? Student { get; set; }
}
