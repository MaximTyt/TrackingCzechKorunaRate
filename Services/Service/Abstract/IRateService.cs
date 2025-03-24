using Domain.DTO;

namespace Services.Service.Abstract
{
    public interface IRateService
    {
        public void EntriesForEachAsync(IEnumerable<KeyValuePair<string, decimal>> syncdata, DateOnly currentDate);
        public void SyncBySchedule(IEnumerable<string> currencies);
        public void SyncByPeriodAsync(string startDate, string endDate, IEnumerable<string> currencies);
        public Task<IEnumerable<Report>> GetReportAsync(string startDate, string endDate, IEnumerable<string> currencies);
    }
}
