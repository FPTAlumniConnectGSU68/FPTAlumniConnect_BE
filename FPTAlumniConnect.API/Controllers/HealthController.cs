using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Health check requested");

            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            });
        }

        [HttpGet("ready")]
        public IActionResult Ready()
        {
            // Add any readiness checks here (database connection, etc.)
            return Ok(new
            {
                Status = "Ready",
                Timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("live")]
        public IActionResult Live()
        {
            // Add any liveness checks here
            return Ok(new
            {
                Status = "Alive",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}