using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.DataTier.Paginate;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.EventTimeLine;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class EventTimeLineController : BaseController<EventTimeLineController>
    {
        private readonly ITimeLineService _Service;

        public EventTimeLineController(ILogger<EventTimeLineController> logger, ITimeLineService service) : base(logger)
        {
            _Service = service;
        }

        [HttpGet(ApiEndPointConstant.TimeLine.TimeLineEndPoint)]
        [ProducesResponseType(typeof(TimeLineReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTimeLineById(int id)
        {
            var response = await _Service.GetTimeLineById(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.TimeLine.TimeLinesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewTimeLine([FromBody] TimeLineInfo request)
        {
            var id = await _Service.CreateTimeLine(request);
            return CreatedAtAction(nameof(GetTimeLineById), new { id }, id);
        }

        [HttpGet(ApiEndPointConstant.TimeLine.TimeLinesEndPoint)]
        [ProducesResponseType(typeof(IPaginate<TimeLineReponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllTimeLine([FromQuery] TimeLineFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _Service.ViewAllTimeLine(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.TimeLine.TimeLineEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTimeLine(int id, [FromBody] TimeLineInfo request)
        {
            var isSuccessful = await _Service.UpdateTimeLine(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}
