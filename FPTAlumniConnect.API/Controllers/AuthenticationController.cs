using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload.User;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class AuthenticationController : BaseController<AuthenticationController>
    {
        private readonly IUserService _userService;

        public AuthenticationController(ILogger<AuthenticationController> logger, IUserService userService) : base(logger)
        {
            _userService = userService;
        }

        //[HttpPost(ApiEndPointConstant.User.UserLoginEndPoint)]
        //[ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        //public async Task<IActionResult> LoginGoogleUser(LoginFirebaseRequest request)
        //{
        //    var response = await _userService.LoginUser(request);
        //    return Ok(response);
        //}
        [HttpPost(ApiEndPointConstant.Authentication.Login)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _userService.Login(request);
            return Ok(response);
        }
        [HttpPost(ApiEndPointConstant.Authentication.Register)]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var response = await _userService.Register(request);
            if (response == null)
            {
                return BadRequest("Registration failed. Please check your input and try again.");
            }

            return CreatedAtAction(nameof(Register), new { id = response.UserId }, response);
        }
        [HttpPost(ApiEndPointConstant.Authentication.GoogleLogin)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GoogleLogin([FromBody] LoginGoogleRequest request)
        {
            try
            {
                var response = await _userService.LoginWithGoogle(request);
                return Ok(response);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}