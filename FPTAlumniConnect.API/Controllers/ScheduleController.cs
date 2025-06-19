using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;

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
        [ProducesResponseType(typeof(ScheduleReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var response = await _scheduleService.GetScheduleById(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Schedule.SchedulesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewSchedule([FromBody] ScheduleInfo request)
        {
            var id = await _scheduleService.CreateNewSchedule(request);
            return CreatedAtAction(nameof(GetScheduleById), new { id }, id);
        }

        [HttpGet(ApiEndPointConstant.Schedule.SchedulesEndPoint)]
        [ProducesResponseType(typeof(ScheduleReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllSchedule([FromQuery] ScheduleFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _scheduleService.ViewAllSchedule(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Schedule.ScheduleEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateScheduleInfo(int id, [FromBody] ScheduleInfo request)
        {
            var isSuccessful = await _scheduleService.UpdateScheduleInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}
