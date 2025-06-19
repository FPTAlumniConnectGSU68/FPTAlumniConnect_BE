using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.Notification;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.DataTier.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : BaseController<NotificationController>
    {
        private readonly INotificationService _notificationService;

        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService) : base(logger)
        {
            _notificationService = notificationService;
        }

        // Get user notifications
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(List<NotificationResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserNotifications(int userId)
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        // Mark a notification as read
        [HttpPatch("mark-as-read/{notificationId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var result = await _notificationService.MarkAsReadAsync(notificationId);
            if (result) return Ok("Notification marked as read.");
            return BadRequest("Failed to mark notification as read.");
        }

        // Send a new notification
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> SendNotification([FromBody] NotificationPayload request)
        {
            var success = await _notificationService.SendNotificationAsync(request);
            if (success)
            {
                return CreatedAtAction(nameof(GetUserNotifications), new { userId = request.UserId }, null);
            }
            return BadRequest("Failed to send notification.");
        }
    }
}
