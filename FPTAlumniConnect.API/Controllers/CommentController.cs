using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.Comment;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class CommentController : BaseController<CommentController>
    {
        private readonly ICommentService _commentService;

        public CommentController(ILogger<CommentController> logger, ICommentService commentService) : base(logger)
        {
            _commentService = commentService;
        }

        [HttpGet(ApiEndPointConstant.Comment.CommentEndPoint)]
        [ProducesResponseType(typeof(CommentReponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommentById(int id)
        {
            var response = await _commentService.GetCommentById(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.Comment.CommentsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewComment([FromBody] CommentInfo request)
        {
            var id = await _commentService.CreateNewComment(request);
            return CreatedAtAction(nameof(GetCommentById), new { id }, id);
        }

        [HttpGet(ApiEndPointConstant.Comment.CommentsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<CommentReponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllComment([FromQuery] CommentFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _commentService.ViewAllComment(filter, pagingModel);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.Comment.CommentEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCommentInfo(int id, [FromBody] CommentInfo request)
        {
            var isSuccessful = await _commentService.UpdateCommentInfo(id, request);
            if (!isSuccessful) return Ok("UpdateCommentFailed");
            return Ok("UpdateCommentSuccess");
        }
    }
}
