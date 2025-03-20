namespace Models.Model
{
    public class RateModel
    {
        public DateOnly Date { get; set; }
        public string Currency { get; set; }
        public decimal RateValue { get; set; }
    }
}
