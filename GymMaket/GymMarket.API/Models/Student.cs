using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class Student
{
    public string StudentId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? HealthStatus { get; set; }

    public string? ProfilePicture { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UserId { get; set; }

    public virtual ICollection<CourseRating> CourseRatings { get; set; } = new List<CourseRating>();

    public virtual ICollection<CourseRegistration> CourseRegistrations { get; set; } = new List<CourseRegistration>();

    public virtual ICollection<HealthDatum> HealthData { get; set; } = new List<HealthDatum>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? AppUser { get; set; }
}
