using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.GroupChat;
using Microsoft.AspNetCore.Mvc;

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
                var id = await _groupChatService.CreateGroupChat(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create group chat");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.GroupChat.GroupChatEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGroupChatById(int id)
        {
            try
            {
                var response = await _groupChatService.GetGroupChatById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch group chat");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPut(ApiEndPointConstant.GroupChat.GroupChatEndPoint)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateGroupChat(int id, [FromBody] GroupChatInfo request)
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
                var isSuccessful = await _groupChatService.UpdateGroupChat(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update group chat");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.GroupChat.GroupChatsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllGroupChats([FromQuery] GroupChatFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _groupChatService.ViewAllMessagesInGroupChat(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch group chat");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.GroupChat.AddUserToGroupEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddUserToGroup(int groupId, int userId)
        {
            var result = await _groupChatService.AddUserToGroup(groupId, userId);
            if (!result) return BadRequest("AddUserFailed");
            return Ok("UserAddedToGroup");
        }

        [HttpDelete(ApiEndPointConstant.GroupChat.LeaveGroupEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LeaveGroup(int groupId, int userId)
        {
            var result = await _groupChatService.LeaveGroup(groupId, userId);
            if (!result) return BadRequest("LeaveFailed");
            return Ok("UserLeftGroup");
        }

        [HttpGet(ApiEndPointConstant.GroupChat.UserGroupsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGroupsByUserId(int userId)
        {
            try
            {
                var response = await _groupChatService.GetGroupsByUserId(userId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch group chat");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.GroupChat.GroupMembersEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMembersInGroup(int groupId)
        {
            try
            {
                var response = await _groupChatService.GetMembersInGroup(groupId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch group chat");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.GroupChat.SearchGroupsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchGroupsByName([FromQuery] string keyword)
        {
            try
            {
                var response = await _groupChatService.SearchGroupsByName(keyword);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch group chat");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}
