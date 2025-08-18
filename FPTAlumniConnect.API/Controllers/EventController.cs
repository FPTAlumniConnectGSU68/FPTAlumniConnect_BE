using FPTAlumniConnect.API.Exceptions;
using FPTAlumniConnect.API.Services.Implements;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Event;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class EventController : BaseController<EventController>
    {
        private readonly IEventService _eventService;

        public EventController(ILogger<EventController> logger, IEventService eventService) : base(logger)
        {
            _eventService = eventService;
        }

        [HttpPost(ApiEndPointConstant.Event.EventsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewEvent([FromBody] EventInfo request)
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
                // 1. Tạo sự kiện mới
                var eventId = await _eventService.CreateNewEvent(request);
                // 2. Kiểm tra và tính toán timeline
                if (!request.StartDate.HasValue || !request.EndDate.HasValue)
                {
                    // Nếu không có thời gian cụ thể, chỉ trả về ID sự kiện
                    return StatusCode(201, new
                    {
                        status = "success",
                        message = "Event created successfully (no timeline suggestions)",
                        data = new { id = eventId }
                    });
                }
                // 3. Tính toán thời lượng sự kiện
                var eventDuration = (request.EndDate.Value - request.StartDate.Value).TotalHours;

                // 4. Lấy gợi ý timeline
                var timelineSuggestions = _eventService.GetSuggestedTimelines(
                    request.StartDate.Value,
                    (int)eventDuration
                );
                // 5. Kiểm tra validation timeline
                if (timelineSuggestions != null && timelineSuggestions.Any(t => t.EndTime > request.EndDate.Value))
                {
                    return StatusCode(201, new
                    {
                        status = "partial_success",
                        message = "Event created. Some timeline suggestions may exceed event end time",
                        data = new
                        {
                            id = eventId,
                            timelineSuggestions
                        }
                    });
                }
                // 6. Trả về thành công
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Event and timeline suggestions created successfully",
                    data = new
                    {
                        id = eventId,
                        timelineSuggestions
                    }
                });
            }

            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    status = "error",
                    message = ex.Message
                });
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create event");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error",
                    detail = ex.Message
                });
            }
        }

        [HttpGet(ApiEndPointConstant.Event.EventEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventById(int id)
        {
            try
            {
                var response = await _eventService.GetEventById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch event");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.Event.EventDetailEndPoint)]
        [ProducesResponseType(typeof(EventDetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventDetail([FromRoute] int id)
        {
            try
            {
                var response = await _eventService.GetEventByIdAsync(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (NotFoundException nf)
            {
                _logger.LogInformation("Event not found: {EventId}", id);
                return NotFound(new { status = "error", message = nf.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch event detail for {EventId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.Event.EventJoinedByUserIdEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventJoinedByUserId(
            [FromRoute] int id,
            [FromQuery] EventFilter filter,
            [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _eventService.GetEventsUserJoined(id, filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch event");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPut(ApiEndPointConstant.Event.EventEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEventInfo(int id, [FromBody] EventInfo request)
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
                var isSuccessful = await _eventService.UpdateEventInfo(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }
                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update event");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }


        [HttpGet(ApiEndPointConstant.Event.EventsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllEvents([FromQuery] EventFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _eventService.ViewAllEvent(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch event");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Lấy danh sách sự kiện theo độ phổ biến
        [HttpGet(ApiEndPointConstant.Event.EventPopularityEndPoint)]
        [ProducesResponseType(typeof(IEnumerable<EventPopularityDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventsByPopularity(int top = 10)
        {
            try
            {
                var response = await _eventService.GetEventsByPopularity(top);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch events by popularity");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Tìm sự kiện tương tự
        [HttpGet(ApiEndPointConstant.Event.EventSimilarEndPoint)]
        [ProducesResponseType(typeof(IEnumerable<GetEventResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSimilarEvents(int eventId, int count = 3)
        {
            try
            {
                var response = await _eventService.GetSimilarEvents(eventId, count);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch similar events");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Kiểm tra sự kiện có bị trùng lịch không
        [HttpGet(ApiEndPointConstant.Event.EventConflictEndPoint)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckEventConflict(int eventId, DateTime newStart, DateTime newEnd)
        {
            try
            {
                var response = await _eventService.CheckEventConflict(eventId, newStart, newEnd);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check event conflict");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.Event.SuggestBestTimeForNewEventEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SuggestBestTimeForNewEvent(int organizerId, int durationHours)
        {
            try
            {
                var response = await _eventService.SuggestBestTimeForNewEvent(organizerId, durationHours);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to suggest best time for new event");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.Event.CountEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CountAllEvents()
        {
            try
            {
                var response = await _eventService.CountAllEvents();
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch event count");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.Event.CountMonthEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CountEventsByMonth(int month, int year)
        {
            try
            {
                var response = await _eventService.CountEventsByMonth(month, year);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch event count by month");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.Event.EventCountByStatusEndPoint)]
        [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEventCountByStatus()
        {
            try
            {
                var response = await _eventService.GetEventCountByStatus();
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch event count by status");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

    }
}
