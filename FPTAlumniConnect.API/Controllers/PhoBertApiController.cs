using FPTAlumniConnect.API.Services.Implements;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class PhoBertApiController : BaseController<PhoBertApiController>
    {
        private readonly IPhoBertService _phoBertService;

        public PhoBertApiController(ILogger<PhoBertApiController> logger, IPhoBertService phoBertService)
            : base(logger)
        {
            _phoBertService = phoBertService;
        }
        [HttpPost(ApiEndPointConstant.PhoBert.FindBestMatchingCVEndpoint)]
        [ProducesResponseType(typeof(int?), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FindBestMatchingCV(int idJobPost)
        {
            var bestCvId = await _phoBertService.RecommendCVForJobPostAsync(idJobPost);
            if (bestCvId == null)
            {
                return NotFound("No matching CV found.");
            }

            return Ok(bestCvId);
        }
    }
}
