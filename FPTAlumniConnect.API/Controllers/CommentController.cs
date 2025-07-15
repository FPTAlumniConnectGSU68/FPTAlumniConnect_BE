using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Comment;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommentById(int id)
        {
            try
            {
                var response = await _commentService.GetCommentById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch comment");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.Comment.CommentsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewComment([FromBody] CommentInfo request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { "Request body is null or malformed" }
                });
            }
            try
            {
                var id = await _commentService.CreateNewComment(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create comment");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.Comment.CommentsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllComment([FromQuery] CommentFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _commentService.ViewAllComment(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch comment");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.Comment.CommentEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCommentInfo(int id, [FromBody] CommentInfo request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { "Request body is null or malformed" }
                });
            }
            try
            {
                var isSuccessful = await _commentService.UpdateCommentInfo(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update comment");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}
