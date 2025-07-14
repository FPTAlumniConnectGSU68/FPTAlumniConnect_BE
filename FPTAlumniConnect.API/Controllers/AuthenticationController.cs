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
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray()
                });
            }

            try
            {
                var response = await _userService.Login(request);

                if (response.AccessToken == null)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = response.Message
                    });
                }

                return Ok(new
                {
                    status = "success",
                    message = response.Message,
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to login");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.Authentication.Register)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterRequest request)
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
                var id = await _userService.Register(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
        [HttpPost(ApiEndPointConstant.Authentication.GoogleLogin)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GoogleLogin([FromBody] LoginGoogleRequest request)
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
                var id = await _userService.LoginWithGoogle(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }

        }
    }
}