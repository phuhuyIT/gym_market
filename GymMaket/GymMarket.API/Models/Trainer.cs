using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class Trainer
{
    public string TrainerId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Certification { get; set; }

    public string? Bio { get; set; }

    public int? Experience { get; set; }

    public decimal? Rating { get; set; }

    public string? ProfilePicture { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UserId { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual AppUser? AppUser { get; set; }
}
