using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class CourseModule
{
    public string ModuleId { get; set; } = null!;

    public string? CourseId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? Order { get; set; }

    public string? PrerequisiteModuleId { get; set; }

    public int? UnlockAfterDays { get; set; }

    public DateTime? AvailableFrom { get; set; }

    public DateTime? AvailableUntil { get; set; }

    public bool IsPublished { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual CourseModule? PrerequisiteModule { get; set; }

    public virtual ICollection<CourseModule> DependentModules { get; set; } = new List<CourseModule>();

    public virtual ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
}
