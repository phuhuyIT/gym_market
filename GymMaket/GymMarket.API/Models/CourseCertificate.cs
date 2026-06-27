namespace GymMarket.API.Models;

public partial class CourseCertificate
{
    public string CertificateId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string VerificationCode { get; set; } = null!;

    public DateTime IssuedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Student? Student { get; set; }
}
