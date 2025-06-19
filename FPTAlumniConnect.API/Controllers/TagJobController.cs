using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.TagJob;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class TagJobController : BaseController<TagJobController>
    {
        private readonly ITagService _tagService;

        public TagJobController(ILogger<TagJobController> logger, ITagService tagService) : base(logger)
        {
            _tagService = tagService;
        }

        [HttpGet(ApiEndPointConstant.Tag.TagEndPoint)]
        [ProducesResponseType(typeof(TagJobReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTagById(int id)
        {
            var response = await _tagService.GetTagById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.Tag.TagCVEndPoint)]
        [ProducesResponseType(typeof(TagJobReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTagByCvId(int id)
        {
            var response = await _tagService.GetTagByCvId(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Tag.TagsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewTag([FromBody] TagJobInfo request)
        {
            var id = await _tagService.CreateNewTag(request);
            return CreatedAtAction(nameof(GetTagById), new { id }, id);
        }

        [HttpGet(ApiEndPointConstant.Tag.TagsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<TagJobReponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllTag([FromQuery] TagJobFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _tagService.ViewAllTag(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Tag.TagEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTagInfo(int id, [FromBody] TagJobInfo request)
        {
            var isSuccessful = await _tagService.UpdateTagInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }
    }
}
