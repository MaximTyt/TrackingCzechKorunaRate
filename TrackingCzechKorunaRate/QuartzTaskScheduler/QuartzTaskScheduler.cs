using Microsoft.Extensions.Options;
using Quartz;
using Settings;
using TrackingCzechKorunaRate.Jobs;

namespace TrackingCzechKorunaRate.QuartzTaskScheduler
{
    public class QuartzTaskScheduler
    {
        private readonly IScheduler _scheduler;
        private readonly IOptionsMonitor<SyncByScheduleSetting> _syncSettings;
        private readonly JobKey _jobKey;
        private readonly TriggerKey _triggerKey;

        public QuartzTaskScheduler(ISchedulerFactory schedulerFactory, IOptionsMonitor<SyncByScheduleSetting> syncSettings)
        {            
            _syncSettings = syncSettings;
            _jobKey = new JobKey("CurrencySyncJob");
            _triggerKey = new TriggerKey("CurrencySyncTrigger");

            _scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            // Подписка на изменения конфигурации
            _syncSettings.OnChange(ReloadTrigger);
        }

        public async Task StartAsync()
        {
            // Создание задачи
            var job = JobBuilder.Create<SyncByScheduleJob>()
                .WithIdentity(_jobKey)
                .StoreDurably()
                .Build();

            // Добавление задачи в планировщик
            await _scheduler.AddJob(job, replace: true);
            // Создание триггера
            var trigger = CreateTrigger();

            // Планирование задачи
            await _scheduler.ScheduleJob(trigger);
        }

        private ITrigger CreateTrigger()
        {
            return TriggerBuilder.Create()
                .WithIdentity(_triggerKey)
                .ForJob(_jobKey)
                .WithCronSchedule(_syncSettings.CurrentValue.CronSсhedule)
                .Build();
        }

        private async void ReloadTrigger(SyncByScheduleSetting settings)
        {
            // Удаление старого триггера
            await _scheduler.UnscheduleJob(_triggerKey);

            // Создание нового триггера
            var newTrigger = CreateTrigger();

            // Планирование задачи с новым триггером
            await _scheduler.ScheduleJob(newTrigger);
        }
    }
}
