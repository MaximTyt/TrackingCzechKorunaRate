namespace Settings
{
    public class SyncByPeriodSetting
    {
        public List<string> Currencies { get; set; }
        public string startDate { get; set; } = "10.03.2025";
        public string endDate { get; set; } = DateTime.UtcNow.ToShortDateString();
    }
}
