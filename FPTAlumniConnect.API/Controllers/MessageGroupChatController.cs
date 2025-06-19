using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.MessageGroupChat;
using FPTAlumniConnect.DataTier.Paginate;
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
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateMessage([FromBody] MessageGroupChatInfo request)
        {
            var messageId = await _messageGroupChatService.CreateMessageGroupChat(request);
            return CreatedAtAction(nameof(GetMessageById), new { id = messageId }, messageId);
        }

        [HttpGet(ApiEndPointConstant.MessageGroupChat.MessageEndPoint)]
        [ProducesResponseType(typeof(MessageGroupChatInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMessageById(int id)
        {
            var messageResponse = await _messageGroupChatService.GetMessageGroupChatById(id);
            return Ok(messageResponse);
        }

        [HttpPut(ApiEndPointConstant.MessageGroupChat.MessageEndPoint)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMessage(int id, [FromBody] MessageGroupChatInfo request)
        {
            bool isUpdated = await _messageGroupChatService.UpdateMessageGroupChat(id, request);
            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet(ApiEndPointConstant.MessageGroupChat.MessagesEndPoint)]
        [ProducesResponseType(typeof(IPaginate<MessageGroupChatInfo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllMessages([FromQuery] MessageGroupChatFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var messages = await _messageGroupChatService.ViewAllMessagesInGroupChat(filter, pagingModel);
            return Ok(messages);
        }
    }
}
