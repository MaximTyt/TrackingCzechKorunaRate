namespace Domain.DTO
{
    public class Rate
    {
        public DateOnly Date { get; set; }
        public string Currency { get; set; }
        public decimal RateValue { get; set; }
    }
}
