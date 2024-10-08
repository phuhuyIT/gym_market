using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class Payment
{
    public string PaymentId { get; set; } = null!;

    public string? CourseId { get; set; }

    public string? StudentId { get; set; }

    public decimal? PaymentAmount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Student? Student { get; set; }
}
