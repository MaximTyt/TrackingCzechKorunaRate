using Data.Repositories.Abstract;
using Data.Repositories.Implementation;
using Domain.DTO;
using Entities.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Service.Abstract;
using Settings;
using System.Buffers.Text;
using System.Net.Http;

namespace Services.Service.Implementation
{
    public class RateService : IRateService
    {
        private static HttpClient sharedClient = new()
        {
            BaseAddress = new Uri("https://www.cnb.cz/en/financial_markets/foreign_exchange_market/exchange_rate_fixing/daily.txt"),
        }; 
        private readonly ILogger<RateService> _logger;
        private IRepository<RateEntity> _rate;        
        public RateService(ILogger<RateService> logger, IRepository<RateEntity> rate)
        {
            _logger = logger;
            _rate = rate;
        }

        public async void SyncBySchedule(IEnumerable<string> currencies)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var data = await GetRateByDateAsync(today.ToShortDateString());
            var syncdata = data.Where(x => currencies.Contains(x.Key));
            foreach (var item in syncdata)
            {
                var entries = await _rate.Find(x => x.Date == today && x.Currency == item.Key);
                var entry = entries.FirstOrDefault();
                if (entry != null)
                {
                    entry.RateValue = item.Value;
                    _rate.Update(entry);
                    _logger.LogInformation($"Updating entry for {item.Key} on {item.Value} for {today}");
                }
                else
                {
                    _rate.Create(new RateEntity
                    {
                        Date = today,
                        Currency = item.Key,
                        RateValue = item.Value
                    });
                    _logger.LogInformation($"Addind entry for {item.Key} on {item.Value} for {today}");
                }
            }
            _logger.LogInformation($"Synchronization by schedule comleted for: {today}");
        }
            

        public async Task<Dictionary<string, decimal>> GetRateByDateAsync(string date)
        {            
            using HttpResponseMessage response = await sharedClient.GetAsync($"?date={date}");
            if (response.IsSuccessStatusCode)
            {
                return Utils.ParseData(response.Content.ReadAsStringAsync());
            }
            return [];
        }

        public async Task<IEnumerable<Report>> GetReportAsync(string startDate, string endDate, IEnumerable<string> currencies)
        {
            DateOnly _currentDate = DateOnly.Parse(startDate);
            DateOnly _endDate = DateOnly.Parse(endDate);
            ICollection<Report> reports = [];
            foreach(var currency in currencies)
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
            }
            if (reports.Count > 0)
            {
                _logger.LogInformation($"Report generated successfully for currencies: {string.Join(", ", currencies)} for period: {startDate} - {endDate}");
            }
            return reports;
        }

        public async void SyncByPeriodAsync(string startDate, string endDate, IEnumerable<string> currencies)
        {
            DateOnly _currentDate = DateOnly.Parse(startDate);
            DateOnly _endDate = DateOnly.Parse(endDate);
            if (_currentDate < _endDate)
            {
                while (_currentDate <= _endDate)
                {
                    var data = await GetRateByDateAsync(_currentDate.ToShortDateString());
                    var syncdata = data.Where(x => currencies.Contains(x.Key));
                    foreach (var item in syncdata)
                    {
                        
                        var entries = await _rate.Find(x => x.Date == _currentDate && x.Currency == item.Key);
                        var entry = entries.FirstOrDefault();
                        if (entry != null)
                        {
                            entry.RateValue = item.Value;
                            _rate.Update(entry);
                            _logger.LogInformation($"Updated entry for {item.Key} on {item.Value} for {_currentDate}");
                        }
                        else
                        {
                            _rate.Create(new RateEntity
                            {
                                Date = _currentDate,
                                Currency = item.Key,
                                RateValue = item.Value
                            });
                            _logger.LogInformation($"Added entry for {item.Key} on {item.Value} for {_currentDate}");
                        }                        
                    }
                    _currentDate = _currentDate.AddDays(1);
                }
                _logger.LogInformation($"Synchronization completed for period: {startDate} - {endDate}");
            }
        }
    }
}
