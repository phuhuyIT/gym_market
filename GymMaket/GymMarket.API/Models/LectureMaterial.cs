using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class LectureMaterial
{
    public string MaterialId { get; set; } = null!;

    public string? LectureId { get; set; }

    public string? MaterialType { get; set; }

    public string? MaterialContent { get; set; }

    public virtual Lecture? Lecture { get; set; }
}
