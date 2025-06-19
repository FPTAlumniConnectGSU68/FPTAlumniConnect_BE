using Azure.Messaging;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.Mentorship;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class MentorshipController : BaseController<MentorshipController>
    {
        private readonly IMentorshipService _mentorshipService;

        public MentorshipController(ILogger<MentorshipController> logger, IMentorshipService mentorshipService) : base(logger)
        {
            _mentorshipService = mentorshipService;
        }

        [HttpGet(ApiEndPointConstant.Mentorship.MentorshipEndPoint)]
        [ProducesResponseType(typeof(MentorshipReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMentorshipById(int id)
        {
            var response = await _mentorshipService.GetMentorshipById(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Mentorship.MentorshipsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewMentorship([FromBody] MentorshipInfo request)
        {
            var id = await _mentorshipService.CreateNewMentorship(request);
            return CreatedAtAction(nameof(GetMentorshipById), new { id }, id);
        }

        [HttpGet(ApiEndPointConstant.Mentorship.MentorshipsEndPoint)]
        [ProducesResponseType(typeof(MentorshipReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllMentorship([FromQuery] MentorshipFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _mentorshipService.ViewAllMentorship(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Mentorship.MentorshipEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMentorshipInfo(int id, [FromBody] MentorshipInfo request)
        {
            var isSuccessful = await _mentorshipService.UpdateMentorshipInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}
