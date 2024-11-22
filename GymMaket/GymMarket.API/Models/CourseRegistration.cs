using System;
using System.Collections.Generic;

namespace GymMarket.API.Models;

public partial class CourseRegistration
{
    public string RegistrationId { get; set; } = null!;

    public string? CourseId { get; set; }

    public string? StudentId { get; set; }

    public string? RegistrationType { get; set; }

    public string? Mode { get; set; }
        
    public string? Status { get; set; }

    public string? PaymentStatus { get; set; }

    public decimal? InitialPayment { get; set; }

    public decimal? AdditionalFeaturesPayment { get; set; }

    public string? ContractAgreement { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<CourseRegistrationOption> CourseRegistrationOptions { get; set; } = new List<CourseRegistrationOption>();

    public virtual Student? Student { get; set; }
}
