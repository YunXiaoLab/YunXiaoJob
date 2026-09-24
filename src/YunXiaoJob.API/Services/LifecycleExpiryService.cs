using YunXiaoJob.Application.Interfaces.Repositories;

namespace YunXiaoJob.API.Services;

public sealed class LifecycleExpiryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopes; private readonly ILogger<LifecycleExpiryService> _logger;
    public LifecycleExpiryService(IServiceScopeFactory scopes, ILogger<LifecycleExpiryService> logger) { _scopes = scopes; _logger = logger; }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopes.CreateScope();
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var jobs = await scope.ServiceProvider.GetRequiredService<IJobPostingRepository>().ExpirePastDeadlineAsync(today, stoppingToken);
                var offers = await scope.ServiceProvider.GetRequiredService<IJobApplicationRepository>().ExpirePastDueOffersAsync(today, stoppingToken);
                if (jobs + offers > 0) _logger.LogInformation("Expired {Jobs} job postings and {Offers} job offers.", jobs, offers);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested) { _logger.LogError(ex, "Lifecycle expiry task failed."); }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
