using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.DataTier.Paginate;
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
        [ProducesResponseType(typeof(IPaginate<GetUserJoinEventResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllUserJoinEvents([FromQuery] UserJoinEventFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var result = await _userJoinEventService.ViewAllUserJoinEvents(filter, pagingModel);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUserJoinEvent([FromBody] UserJoinEventInfo request)
        {
            var userJoinEventId = await _userJoinEventService.CreateNewUserJoinEvent(request);
            return CreatedAtAction(nameof(GetUserJoinEventById), new { id = userJoinEventId }, userJoinEventId);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetUserJoinEventResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserJoinEventById(int id)
        {
            var result = await _userJoinEventService.GetUserJoinEventById(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserJoinEvent(int id, [FromBody] UserJoinEventInfo request)
        {
            var isSuccessful = await _userJoinEventService.UpdateUserJoinEvent(id, request);
            if (!isSuccessful)
            {
                return Ok(new { status = "error", message = "Update failed" });
            }

            return Ok(new { status = "success", message = "Update successful" });
        }
    }
}
