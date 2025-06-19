using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.Event;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.DataTier.Paginate;
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
            [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
            public async Task<IActionResult> CreateNewEvent([FromBody] EventInfo request)
            {
                var eventId = await _eventService.CreateNewEvent(request);
                return CreatedAtAction(nameof(GetEventById), new { id = eventId }, eventId);
            }
            [HttpGet(ApiEndPointConstant.Event.EventEndPoint)]
            [ProducesResponseType(typeof(GetEventResponse), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> GetEventById(int id)
            {
                var eventResponse = await _eventService.GetEventById(id);
                return Ok(eventResponse);
            }
            [HttpPut(ApiEndPointConstant.Event.EventEndPoint)]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> UpdateEventInfo(int id, [FromBody] EventInfo request)
            {
                bool isUpdated = await _eventService.UpdateEventInfo(id, request);
                if (!isUpdated)
                {
                    return NotFound();
                }

                return Ok("UpdateStatusSuccess"); 
            }

            [HttpGet(ApiEndPointConstant.Event.EventsEndPoint)]
            [ProducesResponseType(typeof(IPaginate<GetEventResponse>), StatusCodes.Status200OK)]
            public async Task<IActionResult> ViewAllEvents([FromQuery] EventFilter filter, [FromQuery] PagingModel pagingModel)
            {
                var events = await _eventService.ViewAllEvent(filter, pagingModel);
                return Ok(events);
            }
        }
    }
