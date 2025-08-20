using AutoMapper;
using FPTAlumniConnect.API.Exceptions;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    [Route(ApiEndPointConstant.UserJoinEvent.UserJoinEventsEndPoint)]
    public class UserJoinEventController : ControllerBase
    {
        private readonly IUserJoinEventService _userJoinEventService;
        private readonly ILogger<UserJoinEventController> _logger;
        private readonly IMapper _mapper;

        public UserJoinEventController(IUserJoinEventService userJoinEventService, ILogger<UserJoinEventController> logger, IMapper mapper)
        {
            _userJoinEventService = userJoinEventService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet(ApiEndPointConstant.UserJoinEvent.ViewAllUserJoinEventsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllUserJoinEvents([FromQuery] UserJoinEventFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _userJoinEventService.ViewAllUserJoinEvents(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.UserJoinEvent.UserJoinEventsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUserJoinEvent([FromBody] UserJoinEventInfo request)
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
                var id = await _userJoinEventService.CreateNewUserJoinEvent(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (ConflictException ex)
            {
                return Conflict(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.UserJoinEvent.UserJoinEventEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserJoinEventById(int id)
        {
            try
            {
                var response = await _userJoinEventService.GetUserJoinEventById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPut(ApiEndPointConstant.UserJoinEvent.UserJoinEventEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserJoinEvent(int id, [FromBody] UserJoinEventInfo request)
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
                var isSuccessful = await _userJoinEventService.UpdateUserJoinEvent(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }


        [HttpGet(ApiEndPointConstant.UserJoinEvent.GetTotalParticipantsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalParticipants(int eventId)
        {
            try
            {
                var total = await _userJoinEventService.GetTotalParticipants(eventId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = total
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get total participants");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.UserJoinEvent.GetTotalParticipantsByRoleEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalParticipantsByRole(int eventId)
        {
            try
            {
                var result = await _userJoinEventService.GetTotalParticipantsByRole(eventId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get participants by role");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.UserJoinEvent.GetTotalParticipantsByDayEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalParticipantsByDay(int eventId)
        {
            try
            {
                var result = await _userJoinEventService.GetTotalParticipantsByDay(eventId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get participants by day");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.UserJoinEvent.GetEvaluationsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvaluations(int eventId)
        {
            try
            {
                var result = await _userJoinEventService.GetEvaluations(eventId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get evaluations");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.UserJoinEvent.CheckUserParticipationEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckUserParticipation(int userId, int eventId)
        {
            try
            {
                var isJoined = await _userJoinEventService.CheckUserParticipation(userId, eventId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = isJoined
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check participation");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

    }
}
