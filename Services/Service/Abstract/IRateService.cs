using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service.Abstract
{
    public interface IRateService
    {
        public Task<Dictionary<string, decimal>> GetRateByDateAsync(string date);
        public void SyncBySchedule(IEnumerable<string> currencies);
        public void SyncByPeriodAsync(string startDate, string endDate, IEnumerable<string> currencies);
        public Task<IEnumerable<Report>> GetReportAsync(string startDate, string endDate, IEnumerable<string> currencies);
    }
}
