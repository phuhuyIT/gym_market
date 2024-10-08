using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class Message
{
    public string MessageId { get; set; } = null!;

    public string? CourseId { get; set; }

    public string? StudentId { get; set; }

    public string? TrainerId { get; set; }

    public string? MessageContent { get; set; }

    public string? MessageType { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Student? Student { get; set; }

    public virtual Trainer? Trainer { get; set; }
}
