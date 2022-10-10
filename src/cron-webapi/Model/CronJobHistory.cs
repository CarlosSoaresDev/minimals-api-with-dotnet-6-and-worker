namespace cron_webapi.Model;

internal class CronJobHistory
{
    public Guid JobId { get; set; } = Guid.NewGuid();
    public string ExecutedCronDetails { get; set; }
}

