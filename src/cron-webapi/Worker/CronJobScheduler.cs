using cron_webapi.Service;

namespace cron_webapi.Worker;

internal class CronJobScheduler : BackgroundService
{
    private CrontabSchedule _schedule;
    private DateTime _nextRun;

    private readonly ICronJobService _cronService;    

    public CronJobScheduler(ICronJobService cronService)
    {
        _cronService = cronService;
        SetNextJob();
    }

    private void SetNextJob()
    {
        var schedule = _cronService.GetJobTimer();
        _schedule = CrontabSchedule.Parse(schedule, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
        _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            if (DateTime.Now > _nextRun)
            {
                await _cronService.ExeculteProcessAsync();
                SetNextJob();
            }
            await Task.Delay(5000, stoppingToken);
        }
        while (!stoppingToken.IsCancellationRequested);
    }
}

