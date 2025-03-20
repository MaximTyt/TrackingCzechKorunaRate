using Microsoft.Extensions.Options;
using Models.Model;
using Quartz;
using Services.Service.Abstract;
using Settings;

namespace TrackingCzechKorunaRate.Jobs
{
    public class SyncByScheduleJob : IJob
    {
        private readonly SyncByScheduleSetting _syncSettings;
        private readonly IRateService _rate;
        private readonly ILogger<SyncByScheduleJob> _logger;
        public SyncByScheduleJob(ILogger<SyncByScheduleJob> logger, IOptionsMonitor<SyncByScheduleSetting> syncSettings, IRateService rate)
        {
            _logger = logger;
            _syncSettings = syncSettings.CurrentValue;
            _rate = rate;
        }

        public Task Execute(IJobExecutionContext context)
        {            
            _logger.LogInformation("Scheduled synchronization using a cron-expression: {CronSchedule}", _syncSettings.CronSсhedule);
            _rate.SyncBySchedule(_syncSettings.Currencies);
            return Task.CompletedTask;
        }
    }
}
