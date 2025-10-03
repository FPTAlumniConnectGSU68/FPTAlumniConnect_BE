using FPTAlumniConnect.API.Services.Implements;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class ScheduleController : BaseController<ScheduleController>
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(ILogger<ScheduleController> logger, IScheduleService scheduleService) : base(logger)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet(ApiEndPointConstant.Schedule.ScheduleEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            try
            {
                var response = await _scheduleService.GetScheduleById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch schedule");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.Schedule.AcceptMentorshipEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AcceptMentorShip([FromBody] ScheduleInfo request)
        {
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
                var id = await _scheduleService.AcceptMentorShip(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Mentorship accepted and schedule created successfully",
                    data = new { id }
                });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = ex.Message,
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to accept mentorship or create schedule");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error"
                });
            }
        }

        [HttpPatch(ApiEndPointConstant.Schedule.FailScheduleEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FailSchedule(int id, [FromBody] FailScheduleRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { status = "error", message = "Failure reason is required" });
            }

            try
            {
                var result = await _scheduleService.FailSchedule(id, request.Message);
                if (!result)
                {
                    return BadRequest(new { status = "error", message = "Fail schedule operation failed" });
                }

                return Ok(new { status = "success", message = "Schedule marked as failed" });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning(ex, "Invalid request for FailSchedule");
                return BadRequest(new { status = "error", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark schedule as failed");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }


        [HttpGet(ApiEndPointConstant.Schedule.ScheduleMentorEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetScheduleByMemtorId(int id)
        {
            try
            {
                var response = await _scheduleService.GetSchedulesByMentorId(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch schedule");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.Schedule.SchedulesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewSchedule([FromBody] ScheduleInfo request)
        {
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
                var id = await _scheduleService.CreateNewSchedule(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (BadHttpRequestException ex)
            {
                // Handle specific service-layer validation errors
                return BadRequest(new
                {
                    status = "error",
                    message = ex.Message, // Use the specific error message from the service
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                _logger.LogError(ex, "Failed to create schedule");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error"
                });
            }
        }

        [HttpGet(ApiEndPointConstant.Schedule.SchedulesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllSchedule([FromQuery] ScheduleFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _scheduleService.ViewAllSchedule(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch schedule");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.Schedule.CountEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CountAllSchedules()
        {
            try
            {
                var response = await _scheduleService.CountAllSchedules();
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch schedule count");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.Schedule.CountMonthEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CountSchedulesByMonth(int month, int year)
        {
            try
            {
                var response = await _scheduleService.CountSchedulesByMonth(month,year);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch schedule count by month");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.Schedule.ScheduleEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateScheduleInfo(int id, [FromBody] ScheduleInfo request)
        {
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
                var isSuccessful = await _scheduleService.UpdateScheduleInfo(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update schedule");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.Schedule.CompleteScheduleEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CompleteSchedule(int id)
        {
            try
            {
                var isSuccessful = await _scheduleService.CompleteSchedule(id);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Completion failed" });
                }

                return Ok(new { status = "success", message = "Schedule and mentorship status updated to Completed" });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = ex.Message,
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete schedule or update mentorship status");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
        [HttpPost(ApiEndPointConstant.Schedule.ScheduleRateMentorEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RateMentor([FromRoute] int scheduleId, [FromBody] RateMentorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Comment) || request.Rate < 1 || request.Rate > 5)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { "Invalid rating data" }
                });
            }

            try
            {
                var success = await _scheduleService.RateMentor(scheduleId, request.Comment, request.Rate);

                if (!success)
                {
                    return Ok(new
                    {
                        status = "error",
                        message = "Rating failed"
                    });
                }

                return Ok(new
                {
                    status = "success",
                    message = "Mentor rated successfully"
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning(ex, "Invalid request");
                return BadRequest(new
                {
                    status = "error",
                    message = ex.Message,
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rate mentor");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error"
                });
            }
        }

    }
}
