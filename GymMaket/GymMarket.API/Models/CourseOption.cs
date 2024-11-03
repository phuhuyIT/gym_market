using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class CourseOption
{
    public string OptionId { get; set; } = null!;

    public string? OptionName { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<CourseRegistrationOption> CourseRegistrationOptions { get; set; } = new List<CourseRegistrationOption>();
    public virtual ICollection<CourseRating> CourseRatings { get; set; } = new List<CourseRating>();
}
