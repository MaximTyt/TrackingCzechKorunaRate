using Microsoft.AspNetCore.Mvc;
using Models.Model;
using Mapper;
using Services.Service.Abstract;
using System.Collections.Generic;
using Settings;
using Microsoft.Extensions.Options;

namespace TrackingCzechKorunaRate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RateController : Controller
    {
        private IRateService _rate;
        private readonly SyncByPeriodSetting _syncByPeriodSetting;        
        public RateController(IRateService rate, IOptionsMonitor<SyncByPeriodSetting> syncByPeriodSetting)
        {
            _rate = rate;
            _syncByPeriodSetting = syncByPeriodSetting.CurrentValue;
        }

        [HttpPost("syncbyperiod")]
        public void SyncByPeriod(string startDate, string endDate = "")
        {
            endDate = endDate == "" ? DateTime.UtcNow.ToShortDateString() : endDate;
            _rate.SyncByPeriodAsync(startDate, endDate, _syncByPeriodSetting.Currencies);
        }
        //public void SyncByPeriod([FromBody] SyncByPeriodModel date)
        //{
        //    _rate.SyncByPeriodAsync(date.startDate, date.endDate);
        //}

        [HttpGet("report")]
        public async Task<IEnumerable<ReportModel>> GetReportAsync(string startDate, string endDate, [FromQuery] IEnumerable<string> currencies)
        {            
            var report = await _rate.GetReportAsync(startDate, endDate, currencies);
            return report.Select(x => x.ToModel());
        }
    }
}
