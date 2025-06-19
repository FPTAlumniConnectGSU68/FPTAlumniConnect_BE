using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.SpMajorCode;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class SpMajorCodeController : BaseController<SpMajorCodeController>
    {
        private readonly ISpMajorCodeService _spMajorCodeService;

        public SpMajorCodeController(ILogger<SpMajorCodeController> logger, ISpMajorCodeService spMajorCodeService) : base(logger)
        {
            _spMajorCodeService = spMajorCodeService;
        }

        [HttpGet(ApiEndPointConstant.SpMajorCode.SpMajorCodesEndPoint)]
        [ProducesResponseType(typeof(SpMajorCodeResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSpMajorCodeById(int id)
        {
            var response = await _spMajorCodeService.GetSpMajorCodeById(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.SpMajorCode.SpMajorCodeEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewSpMajorCode([FromBody] SpMajorCodeInfo request)
        {
            var id = await _spMajorCodeService.CreateNewSpMajorCode(request);
            return CreatedAtAction(nameof(GetSpMajorCodeById), new { id }, id);
        }

        [HttpGet(ApiEndPointConstant.SpMajorCode.SpMajorCodeEndPoint)]
        [ProducesResponseType(typeof(IPaginate<SpMajorCodeResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllSpMajorCodes([FromQuery] SpMajorCodeFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _spMajorCodeService.ViewAllSpMajorCodes(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.SpMajorCode.SpMajorCodesEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSpMajorCodeInfo(int id, [FromBody] SpMajorCodeInfo request)
        {
            var isSuccessful = await _spMajorCodeService.UpdateSpMajorCodeInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }

}
