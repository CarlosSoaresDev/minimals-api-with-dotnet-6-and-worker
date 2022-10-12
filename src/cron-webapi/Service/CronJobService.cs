using cron_webapi.Model;

namespace cron_webapi.Service;

internal class CronJobService : ICronJobService
{
    private const string CACHE_HITORY = "historic";
    private const string CACHE_SCHEDULER_TIMER = "schedule_timer";
    private const string DEFAULT_TIMER_JOB = "*/10 * * * * *";

    private readonly ILogger<CronJobHistory> _logger;
    private readonly IDistributedCache _cache;

    public CronJobService(IDistributedCache cache, ILogger<CronJobHistory> logger)
    {
        _logger = logger;
        _cache = cache;
    }

    public string GetJobTimer()
    {
        try
        {
            var cache = _cache.GetString(CACHE_SCHEDULER_TIMER);

            if (string.IsNullOrEmpty(cache))
                _cache.SetString(CACHE_SCHEDULER_TIMER, DEFAULT_TIMER_JOB);

            return _cache.GetString(CACHE_SCHEDULER_TIMER);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<List<CronJobHistory>> GetCronHistoricAsync()
    {
        try
        {
            var cronHistorics = new List<CronJobHistory>();
            var cache = await _cache.GetStringAsync(CACHE_HITORY);

            if (string.IsNullOrEmpty(cache))
                return cronHistorics.ToList();

            return JsonSerializer.Deserialize<List<CronJobHistory>>(cache);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task ExeculteProcessAsync()
    {
        try
        {
            var cache = await _cache.GetStringAsync(CACHE_HITORY);
            var cronHistoryList = new List<CronJobHistory>();

            if (!string.IsNullOrEmpty(cache))
                cronHistoryList = JsonSerializer.Deserialize<List<CronJobHistory>>(cache);

            cronHistoryList.Add(new CronJobHistory { ExecutedCronDetails = $"Execultado {DateTime.Now.ToString("dd/MM/yyyy")} as {DateTime.Now.ToString("HH:mm:ss")}" });
            await _cache.SetStringAsync(CACHE_HITORY, JsonSerializer.Serialize(cronHistoryList));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task UpdateJobTimerAsync(string timer)
    {
        try
        {
            var cache = await _cache.GetStringAsync(CACHE_SCHEDULER_TIMER);

            if (string.IsNullOrEmpty(cache) && string.IsNullOrEmpty(timer))
                await _cache.SetStringAsync(CACHE_SCHEDULER_TIMER, DEFAULT_TIMER_JOB);
            else
                await _cache.SetStringAsync(CACHE_SCHEDULER_TIMER, timer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task DeleteHistoricAsync(Guid id)
    {
        try
        {
            var cache = await _cache.GetStringAsync(CACHE_HITORY);

            if (string.IsNullOrEmpty(cache))
                return;

            var historics = JsonSerializer.Deserialize<List<CronJobHistory>>(cache);
            var historic = historics?.FirstOrDefault(f => f.JobId == id);

            if (historic is null)
                throw new ArgumentNullException("não existe nenhum registro com o id selecionado.");

            historics.Remove(historic);

            await _cache.SetStringAsync(CACHE_HITORY, JsonSerializer.Serialize(historics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task CleanHistoricAsync()
    {
        try
        {
            _cache.Remove(CACHE_HITORY);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}

