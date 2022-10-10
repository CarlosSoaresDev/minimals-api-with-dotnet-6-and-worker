using cron_webapi.Model;

namespace cron_webapi.Service;

internal interface ICronJobService
{
    Task<List<CronJobHistory>> GetCronHistoricAsync();
    Task ExeculteProcessAsync();
    Task UpdateJobTimerAsync(string timer);
    string GetJobTimer();
    Task DeleteHistoricAsync(Guid id);
    Task CleanHistoricAsync();
}