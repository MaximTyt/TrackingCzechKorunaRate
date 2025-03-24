using Data.Repositories.Abstract;
using Domain.DTO;
using Entities.Entity;
using Microsoft.Extensions.Logging;
using Services.Service.Abstract;
using Services.Utils;

namespace Services.Service.Implementation
{
    public class RateService : IRateService
    {         
        private readonly ILogger<RateService> _logger;
        private IRepository<RateEntity> _rate;
        private ICNBRequest _cNBRequest;
        public RateService(ILogger<RateService> logger, IRepository<RateEntity> rate, ICNBRequest cNBRequest)
        {
            _logger = logger;
            _rate = rate;
            _cNBRequest = cNBRequest;
        }

        public async void EntriesForEachAsync(IEnumerable<KeyValuePair<string, decimal>> syncdata, DateOnly currentDate)
        {
            await Parallel.ForEachAsync(syncdata, async (item, token) =>
            {
                var entries = await _rate.Find(x => x.Date == currentDate && x.Currency == item.Key);
                var entry = entries.FirstOrDefault();
                if (entry != null)
                {
                    entry.RateValue = item.Value;
                    _rate.Update(entry);
                    _logger.LogInformation($"Updating entry for {item.Key} on {item.Value} for {currentDate}");
                }
                else
                {
                    _rate.Create(new RateEntity
                    {
                        Date = currentDate,
                        Currency = item.Key,
                        RateValue = item.Value
                    });
                    _logger.LogInformation($"Addind entry for {item.Key} on {item.Value} for {currentDate}");
                }
            });
        }

        public async void SyncBySchedule(IEnumerable<string> currencies)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var data = await _cNBRequest.GetRateByDateAsync(today.ToShortDateString());
            var syncdata = data.Where(x => currencies.Contains(x.Key));
            EntriesForEachAsync(syncdata, today);
            _logger.LogInformation($"Synchronization by schedule comleted for: {today}");
        }       

        public async void SyncByPeriodAsync(string startDate, string endDate, IEnumerable<string> currencies)
        {
            DateOnly _currentDate = DateOnly.Parse(startDate);
            DateOnly _endDate = DateOnly.Parse(endDate);
            if (_currentDate < _endDate)
            {
                while (_currentDate <= _endDate)
                {
                    var data = await _cNBRequest.GetRateByDateAsync(_currentDate.ToShortDateString());
                    var syncdata = data.Where(x => currencies.Contains(x.Key));
                    EntriesForEachAsync(syncdata, _currentDate);
                    _currentDate = _currentDate.AddDays(1);
                }
                _logger.LogInformation($"Synchronization completed for period: {startDate} - {endDate}");
            }
        }
        public async Task<IEnumerable<Report>> GetReportAsync(string startDate, string endDate, IEnumerable<string> currencies)
        {
            DateOnly _currentDate = DateOnly.Parse(startDate);
            DateOnly _endDate = DateOnly.Parse(endDate);
            ICollection<Report> reports = [];
            await Parallel.ForEachAsync(currencies, async (currency, token) =>
            {
                var entries = await _rate.Find(x => x.Currency == currency && x.Date >= _currentDate && x.Date <= _endDate);
                if (entries.Count > 0)
                {
                    reports.Add(new Report
                    {
                        Currency = currency,
                        MinRate = entries.Min(x => x.RateValue),
                        MaxRate = entries.Max(x => x.RateValue),
                        AvgRate = entries.Average(x => x.RateValue)
                    });
                }
                else
                {
                    _logger.LogWarning($"No entries found for currency: {currency} for period : {startDate} - {endDate}");
                }
            });
            if (reports.Count > 0)
            {
                _logger.LogInformation($"Report generated successfully for currencies: {string.Join(", ", currencies)} for period: {startDate} - {endDate}");
            }
            else
            {
                _logger.LogWarning($"The report could not be compiled because there is no data for the currency: {string.Join(", ", currencies)} for period: {startDate} - {endDate}");
            }
            return reports;
        }
    }
}
