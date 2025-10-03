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
    public class MentorshipCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MentorshipCleanupService> _logger;
        private TimeSpan _interval;

        public MentorshipCleanupService(
            IServiceProvider serviceProvider,
            ILogger<MentorshipCleanupService> logger,
            IOptions<MentorshipCleanupOptions> options)
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
            _logger.LogInformation($"MentorshipCleanupService interval updated to {newInterval.TotalHours} hours.");
        }

        // Gets the current cleanup interval
        public TimeSpan GetInterval()
        {
            return _interval;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MentorshipCleanupService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var scheduleService = scope.ServiceProvider.GetRequiredService<IMentorshipService>();
                        int updatedCount = await scheduleService.AutoCancelExpiredMentorships();
                        _logger.LogInformation($"AutoCancelExpiredMentorships updated {updatedCount} mentorships.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while auto-canceling expired mentorships.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("MentorshipCleanupService is stopping.");
        }
    }

    public class MentorshipCleanupOptions
    {
        public TimeSpan Interval { get; set; } = TimeSpan.FromHours(12);
    }
}