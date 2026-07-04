namespace GymMarket.API.DTOs.Certificate;

public class CourseCertificateDto
{
    public string CertificateId { get; set; } = string.Empty;

    public string CourseId { get; set; } = string.Empty;

    public string? CourseTitle { get; set; }

    public string StudentId { get; set; } = string.Empty;

    public string? StudentName { get; set; }

    public string VerificationCode { get; set; } = string.Empty;

    public DateTime IssuedAt { get; set; }

    public CourseCertificateSettingDto? Setting { get; set; }
}
