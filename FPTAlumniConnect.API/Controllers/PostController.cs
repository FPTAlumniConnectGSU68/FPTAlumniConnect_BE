using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.Post;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class PostController : BaseController<PostController>
    {
        private readonly IPostService _postService;

        public PostController(ILogger<PostController> logger, IPostService postService) : base(logger)
        {
            _postService = postService;
        }

        [HttpGet(ApiEndPointConstant.Post.PostEndPoint)]
        [ProducesResponseType(typeof(PostReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostById(int id)
        {
            var response = await _postService.GetPostById(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Post.PostsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewPost([FromBody] PostInfo request)
        {
            var id = await _postService.CreateNewPost(request);
            return CreatedAtAction(nameof(GetPostById), new { id }, id);
        }

        [HttpGet(ApiEndPointConstant.Post.PostsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<PostReponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllPost([FromQuery] PostFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _postService.ViewAllPost(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Post.PostEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePostInfo(int id, [FromBody] PostInfo request)
        {
            var isSuccessful = await _postService.UpdatePostInfo(id, request);
            if (!isSuccessful)
            {
                return Ok(new { status = "error", message = "Update failed" });
            }

            return Ok(new { status = "success", message = "Update successful" });
        }
    }
}
