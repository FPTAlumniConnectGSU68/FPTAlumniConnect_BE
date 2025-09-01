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
        public async Task<IActionResult> FindBestMatchingCV(int idJobPost, [FromQuery] PagingModel pagingModel)
        {
            var bestCvId = await _phoBertService.RecommendCVForJobPostAsync(idJobPost, pagingModel);
            if (bestCvId == null)
            {
                return NotFound("No matching CV found.");
            }

            return Ok(bestCvId);
        }

        [HttpGet(ApiEndPointConstant.PhoBert.RecommendJobsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RecommendJobsForCV(int cvId, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _phoBertService.RecommendJobForCVAsync(cvId, pagingModel);
                return Ok(response);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogWarning(ex, "Failed to recommend jobs for CV ID: {CvId}", cvId);
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to recommend jobs for CV ID: {CvId}", cvId);
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error"
                });
            }
        }
    }
}
