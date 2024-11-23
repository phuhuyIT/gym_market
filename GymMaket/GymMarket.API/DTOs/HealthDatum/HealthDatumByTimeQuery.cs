namespace GymMarket.API.DTOs.HealthDatum
{
    public class HealthDatumByTimeQuery
    {
        public string StudentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
