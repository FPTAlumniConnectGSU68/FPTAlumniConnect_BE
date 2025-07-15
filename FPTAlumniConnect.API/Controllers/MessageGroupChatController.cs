using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.MessageGroupChat;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class MessageGroupChatController : BaseController<MessageGroupChatController>
    {
        private readonly IMessageGroupChatService _messageGroupChatService;

        public MessageGroupChatController(ILogger<MessageGroupChatController> logger, IMessageGroupChatService messageGroupChatService)
            : base(logger)
        {
            _messageGroupChatService = messageGroupChatService;
        }

        [HttpPost(ApiEndPointConstant.MessageGroupChat.MessagesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMessage([FromBody] MessageGroupChatInfo request)
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
                var id = await _messageGroupChatService.CreateMessageGroupChat(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create message");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.MessageGroupChat.MessageEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMessageById(int id)
        {
            try
            {
                var response = await _messageGroupChatService.GetMessageGroupChatById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch message");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPut(ApiEndPointConstant.MessageGroupChat.MessageEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMessage(int id, [FromBody] MessageGroupChatInfo request)
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
                var isSuccessful = await _messageGroupChatService.UpdateMessageGroupChat(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update message");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.MessageGroupChat.MessagesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllMessages([FromQuery] MessageGroupChatFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _messageGroupChatService.ViewAllMessagesInGroupChat(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch message");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}
