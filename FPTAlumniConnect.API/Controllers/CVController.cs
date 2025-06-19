using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class CVController : BaseController<CVController>
    {
        private readonly ICVService _cVService;

        public CVController(ILogger<CVController> logger, ICVService cVService) : base(logger)
        {
            _cVService = cVService;
        }

        [HttpGet(ApiEndPointConstant.CV.CVEndPoint)]
        [ProducesResponseType(typeof(CVReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCVById(int id)
        {
            var response = await _cVService.GetCVById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.CV.CVUserEndPoint)]
        [ProducesResponseType(typeof(CVReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCVByUserId(int id)
        {
            var response = await _cVService.GetCVByUserId(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.CV.CVsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewCV([FromBody] CVInfo request)
        {
            var id = await _cVService.CreateNewCV(request);
            return CreatedAtAction(nameof(GetCVById), new { id }, id);
        }

        [HttpGet(ApiEndPointConstant.CV.CVsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<CVReponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllCV([FromQuery] CVFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _cVService.ViewAllCV(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.CV.CVEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCVInfo(int id, [FromBody] CVInfo request)
        {
            var isSuccessful = await _cVService.UpdateCVInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}
