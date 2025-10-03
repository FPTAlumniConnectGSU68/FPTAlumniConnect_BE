using FPTAlumniConnect.API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FPTAlumniConnect.API.Services
{
    public class JobPostCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobPostCleanupService> _logger;
        private TimeSpan _interval;

        public JobPostCleanupService(
            IServiceProvider serviceProvider,
            ILogger<JobPostCleanupService> logger,
            IOptions<JobPostCleanupOptions> options)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _interval = options.Value.Interval;
        }

        // Updates the cleanup interval at runtime
        public void UpdateInterval(TimeSpan newInterval)
        {
            if (newInterval <= TimeSpan.Zero)
            {
                throw new ArgumentException("Interval must be greater than zero.", nameof(newInterval));
            }
            _interval = newInterval;
            _logger.LogInformation($"JobPostCleanupService interval updated to {newInterval.TotalHours} hours.");
        }

        // Gets the current cleanup interval
        public TimeSpan GetInterval()
        {
            return _interval;
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

    public class JobPostCleanupOptions
    {
        public TimeSpan Interval { get; set; } = TimeSpan.FromHours(4);
    }
}