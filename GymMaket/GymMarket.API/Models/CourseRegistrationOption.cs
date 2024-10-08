using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class CourseRegistrationOption
{
    public string RegistrationOptionId { get; set; } = null!;

    public string? RegistrationId { get; set; }

    public string? OptionId { get; set; }

    public virtual CourseOption? Option { get; set; }

    public virtual CourseRegistration? Registration { get; set; }
}
