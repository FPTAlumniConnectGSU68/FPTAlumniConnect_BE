using FPTAlumniConnect.API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FPTAlumniConnect.API.Services
{
    public class JobPostCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobPostCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(4); // every 12 hours

        public JobPostCleanupService(IServiceProvider serviceProvider, ILogger<JobPostCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("JobPostCleanupService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var jobPostService = scope.ServiceProvider.GetRequiredService<IJobPostService>();
                    int updatedCount = await jobPostService.AutoCloseExpiredJobPosts();
                    _logger.LogInformation("AutoCloseExpiredJobPosts updated {Count} job posts.", updatedCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while auto-closing job posts.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("JobPostCleanupService is stopping.");
        }
    }
}
