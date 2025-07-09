using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.GroupChat;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.DataTier.Paginate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FPTAlumniConnect.BusinessTier.Payload.User;

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

        //[HttpDelete(ApiEndPointConstant.GroupChat.GroupChatEndPoint)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public async Task<IActionResult> DeleteGroupChat(int id)
        //{
        //    var result = await _groupChatService.DeleteGroupChat(id);
        //    if (!result) return NotFound();
        //    return NoContent();
        //}

        [HttpPost(ApiEndPointConstant.GroupChat.AddUserToGroupEndPoint)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddUserToGroup(int groupId, int userId)
        {
            var result = await _groupChatService.AddUserToGroup(groupId, userId);
            if (!result) return BadRequest("AddUserFailed");
            return Ok("UserAddedToGroup");
        }

        [HttpDelete(ApiEndPointConstant.GroupChat.LeaveGroupEndPoint)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LeaveGroup(int groupId, int userId)
        {
            var result = await _groupChatService.LeaveGroup(groupId, userId);
            if (!result) return BadRequest("LeaveFailed");
            return Ok("UserLeftGroup");
        }

        [HttpGet(ApiEndPointConstant.GroupChat.UserGroupsEndPoint)]
        [ProducesResponseType(typeof(List<GroupChatReponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGroupsByUserId(int userId)
        {
            var groups = await _groupChatService.GetGroupsByUserId(userId);
            return Ok(groups);
        }

        [HttpGet(ApiEndPointConstant.GroupChat.GroupMembersEndPoint)]
        [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMembersInGroup(int groupId)
        {
            var members = await _groupChatService.GetMembersInGroup(groupId);
            return Ok(members);
        }

        [HttpGet(ApiEndPointConstant.GroupChat.SearchGroupsEndPoint)]
        [ProducesResponseType(typeof(List<GroupChatReponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchGroupsByName([FromQuery] string keyword)
        {
            var results = await _groupChatService.SearchGroupsByName(keyword);
            return Ok(results);
        }
    }
}
