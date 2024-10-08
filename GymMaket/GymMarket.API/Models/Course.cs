using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class Course
{
    public string CourseId { get; set; } = null!;

    public string? TrainerId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }

    public string? Category { get; set; }

    public decimal? Price { get; set; }

    public decimal? AdditionalPrice { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? Duration { get; set; }

    public int? MaxParticipants { get; set; }

    public decimal? Rating { get; set; }

    public virtual ICollection<CourseRating> CourseRatings { get; set; } = new List<CourseRating>();

    public virtual ICollection<CourseRegistration> CourseRegistrations { get; set; } = new List<CourseRegistration>();

    public virtual ICollection<HealthDatum> HealthData { get; set; } = new List<HealthDatum>();

    public virtual ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Trainer? Trainer { get; set; }
}
