namespace GymMarket.API.DTOs.Momo
{
    // The Base64-encoded JSON we round-trip through Momo's extraData field so the
    // callback/IPN can tell which student paid for which course.
    public class MomoExtraData
    {
        public string? StudentId { get; set; }
        public string? CourseId { get; set; }
    }
}
