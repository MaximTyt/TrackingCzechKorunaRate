namespace Domain.DTO
{
    public class Report
    {
        public string Currency { get; set; }
        public decimal MinRate { get; set; }
        public decimal MaxRate { get; set; }
        public decimal AvgRate { get; set; }
    }
}
