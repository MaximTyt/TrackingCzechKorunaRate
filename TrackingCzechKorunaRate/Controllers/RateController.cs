using Mapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models.Model;
using Services.Service.Abstract;
using Settings;

namespace TrackingCzechKorunaRate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RateController : Controller
    {
        private IRateService _rate;
        private readonly SyncSetting _syncSetting;        
        public RateController(IRateService rate, IOptionsMonitor<SyncSetting> syncSetting)
        {
            _rate = rate;
            _syncSetting = syncSetting.CurrentValue;
        }

        [HttpPost("syncbyperiod")]
        public void SyncByPeriod(string startDate, string endDate = "")
        {
            endDate = endDate == "" ? DateTime.UtcNow.ToShortDateString() : endDate;
            _rate.SyncByPeriodAsync(startDate, endDate, _syncSetting.Currencies);
        }        

        [HttpGet("report")]
        public async Task<IEnumerable<ReportModel>> GetReportAsync(string startDate, string endDate, [FromQuery] IEnumerable<string> currencies)
        {            
            var report = await _rate.GetReportAsync(startDate, endDate, currencies);
            return report.Select(x => x.ToModel());
        }
    }
}
