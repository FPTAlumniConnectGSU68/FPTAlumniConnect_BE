using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.MajorCode;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class MajorCodeController : BaseController<MajorCodeController>
    {
        private readonly IMajorCodeService _majorCodeService;

        public MajorCodeController(ILogger<MajorCodeController> logger, IMajorCodeService majorCodeService) : base(logger)
        {
            _majorCodeService = majorCodeService;
        }

        [HttpGet(ApiEndPointConstant.MajorCode.MajorCodeEndPoint)]
        [ProducesResponseType(typeof(MajorCodeReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMajorCodeById(int id)
        {
            var response = await _majorCodeService.GetMajorCodeById(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.MajorCode.MajorCodesEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewMajorCode([FromBody] MajorCodeInfo request)
        {

            var id = await _majorCodeService.CreateNewMajorCode(request);
            return CreatedAtAction(nameof(GetMajorCodeById), new { id }, id);
        }

        [HttpGet(ApiEndPointConstant.MajorCode.MajorCodesEndPoint)]
        [ProducesResponseType(typeof(IPaginate<MajorCodeReponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllMajorCodes([FromQuery] MajorCodeFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _majorCodeService.ViewAllMajorCode(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.MajorCode.MajorCodeEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMajorCodeInfo(int id, [FromBody] MajorCodeInfo request)
        {
            var isSuccessful = await _majorCodeService.UpdateMajorCodeInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [HttpGet(ApiEndPointConstant.MajorCode.NamesEndpoint)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMajorNames()
        {
            var names = await _majorCodeService.GetAllMajorNames();
            return Ok(names);
        }

        // ✅ NEW: Count all major codes
        [HttpGet(ApiEndPointConstant.MajorCode.CountEndpoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CountAllMajorCodes()
        {
            int count = await _majorCodeService.CountMajorCodesAsync();
            return Ok(count);
        }
    }
}
