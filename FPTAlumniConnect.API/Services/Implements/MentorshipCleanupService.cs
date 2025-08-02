using FPTAlumniConnect.API.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FPTAlumniConnect.API.Services
{
    public class MentorshipCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MentorshipCleanupService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(12); // Run every 6 hours

        public MentorshipCleanupService(
            IServiceProvider serviceProvider,
            ILogger<MentorshipCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
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
}