using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.GroupChat;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.DataTier.Paginate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class GroupChatController : BaseController<GroupChatController>
    {
        private readonly IGroupChatService _groupChatService;

        public GroupChatController(ILogger<GroupChatController> logger, IGroupChatService groupChatService) : base(logger)
        {
            _groupChatService = groupChatService;
        }

        [HttpPost(ApiEndPointConstant.GroupChat.GroupChatsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateGroupChat([FromBody] GroupChatInfo request)
        {
            var groupId = await _groupChatService.CreateGroupChat(request);
            return CreatedAtAction(nameof(GetGroupChatById), new { id = groupId }, groupId);
        }

        [HttpGet(ApiEndPointConstant.GroupChat.GroupChatEndPoint)]
        [ProducesResponseType(typeof(GroupChatReponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGroupChatById(int id)
        {
            var groupChatResponse = await _groupChatService.GetGroupChatById(id);
            return Ok(groupChatResponse);
        }

        [HttpPut(ApiEndPointConstant.GroupChat.GroupChatEndPoint)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateGroupChat(int id, [FromBody] GroupChatInfo request)
        {
            bool isUpdated = await _groupChatService.UpdateGroupChat(id, request);
            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet(ApiEndPointConstant.GroupChat.GroupChatsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<GroupChatReponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllGroupChats([FromQuery] GroupChatFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var groupChats = await _groupChatService.ViewAllMessagesInGroupChat(filter, pagingModel);
            return Ok(groupChats);
        }
    }
}
