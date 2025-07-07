using Azure.Messaging;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.User;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class UserController : BaseController<UserController>
    {
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService) : base(logger)
        {
            _userService = userService;
        }

        [HttpGet(ApiEndPointConstant.User.UserEndPoint)]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser(int id)
        {
            var response = await _userService.GetUserById(id);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.User.UsersEndPoint)]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllUser([FromQuery] UserFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _userService.ViewAllUser(filter, pagingModel);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.User.MentorsEndPoint)]
        [ProducesResponseType(typeof(GetMentorResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllMentor([FromQuery] MentorFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _userService.ViewAllMentor(filter, pagingModel);
            return Ok(response);
        }

        [HttpGet(ApiEndPointConstant.User.MentorRatingEndPoint)]
        [ProducesResponseType(typeof(GetMentorResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAverageRatingByMentorId(int id)
        {
            var response = await _userService.GetAverageRatingByMentorId(id);
            return Ok(response);
        }

        [HttpPatch(ApiEndPointConstant.User.UserEndPoint)]
        [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserInfo(int id, UserInfo request)
        {
            var isSuccessful = await _userService.UpdateUserInfo(id, request);
            if (!isSuccessful) return Ok("UpdateUserFailed");
            return Ok("UpdateUserSuccess");
        }


    }
}
