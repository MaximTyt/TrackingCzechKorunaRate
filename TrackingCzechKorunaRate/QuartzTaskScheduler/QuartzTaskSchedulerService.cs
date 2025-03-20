namespace TrackingCzechKorunaRate.QuartzTaskScheduler
{
    public class QuartzTaskSchedulerService : IHostedService
    {
        private readonly QuartzTaskScheduler _taskScheduler;

        public QuartzTaskSchedulerService(QuartzTaskScheduler taskScheduler)
        {
            _taskScheduler = taskScheduler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _taskScheduler.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
