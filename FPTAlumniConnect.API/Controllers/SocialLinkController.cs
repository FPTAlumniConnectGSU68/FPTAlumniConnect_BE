using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.SocialLink;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class SocialLinkController : BaseController<SocialLinkController>
    {
        private readonly ISocialLinkService _socialLinkService;

        public SocialLinkController(ILogger<SocialLinkController> logger, ISocialLinkService socialLinkService) : base(logger)
        {
            _socialLinkService = socialLinkService;
        }

        [HttpGet(ApiEndPointConstant.SocialLink.SocialLinkEndPoint)]
        [ProducesResponseType(typeof(GetSocialLinkResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSocialLink(int id)
        {
            var response = await _socialLinkService.GetSocialLinkById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.SocialLink.SocialLinksEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GetSocialLinkResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllSocialLinks([FromQuery] SocialLinkFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _socialLinkService.ViewAllSocialLinks(filter, pagingModel);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.SocialLink.SocialLinksEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateSocialLink([FromBody] SocialLinkInfo request)
        {
            var id = await _socialLinkService.CreateSocialLink(request);
            return CreatedAtAction(nameof(GetSocialLink), new { id }, id);
        }

        [HttpPatch(ApiEndPointConstant.SocialLink.SocialLinkEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSocialLink(int id, [FromBody] SocialLinkInfo request)
        {
            var isSuccessful = await _socialLinkService.UpdateSocialLink(id, request);
            if (!isSuccessful) return Ok("UpdateFailed");
            return Ok("UpdateSuccess");
        }

        [HttpDelete(ApiEndPointConstant.SocialLink.SocialLinkEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteSocialLink(int id)
        {
            var isSuccessful = await _socialLinkService.DeleteSocialLink(id);
            if (!isSuccessful) return Ok("DeleteFailed");
            return Ok("DeleteSuccess");
        }
    }
}