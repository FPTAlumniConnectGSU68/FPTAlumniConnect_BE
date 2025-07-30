using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload.Notification;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        //[ProducesResponseType(typeof(List<NotificationResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserNotifications(int userId)
        {
            //var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            //return Ok(notifications);
            try
            {
                var response = await _notificationService.GetUserNotificationsAsync(userId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch comment");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Mark a notification as read
        [HttpPatch("mark-as-read/{notificationId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAsRead(int notificationId, [FromBody] NotificationPayload request)
        {
            //var result = await _notificationService.MarkAsReadAsync(notificationId);
            //if (result) return Ok("Notification marked as read.");
            //return BadRequest("Failed to mark notification as read.");
            if (request == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { "Request body is null or malformed" }
                });
            }
            try
            {
                var isSuccessful = await _notificationService.MarkAsReadAsync(notificationId);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update comment");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Send a new notification
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> SendNotification([FromBody] NotificationPayload request)
        {
            //var success = await _notificationService.SendNotificationAsync(request);
            //if (success)
            //{
            //    return CreatedAtAction(nameof(GetUserNotifications), new { userId = request.UserId }, null);
            //}
            //return BadRequest("Failed to send notification.");
            if (request == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { "Request body is null or malformed" }
                });
            }
            try
            {
                var isSuccessful = await _notificationService.SendNotificationAsync(request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update comment");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}
