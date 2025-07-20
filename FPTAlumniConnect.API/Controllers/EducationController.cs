using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Education;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class EducationController : BaseController<EducationController>
    {
        private readonly IEducationService _educationService;

        public EducationController(ILogger<EducationController> logger, IEducationService educationService) : base(logger)
        {
            _educationService = educationService;
        }

        // POST api/education
        [HttpPost(ApiEndPointConstant.Education.EducationsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEducation([FromBody] EducationInfo request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { "Invalid education data." }
                });
            }

            try
            {
                int educationId = await _educationService.CreateEducationAsync(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id = educationId }
                });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // GET api/education/{id}
        [HttpGet(ApiEndPointConstant.Education.EducationEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEducationById(int id)
        {
            try
            {
                var response = await _educationService.GetEducationByIdAsync(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (BadHttpRequestException ex)
            {
                return NotFound(new { status = "error", message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // PUT api/education/{id}
        [HttpPut(ApiEndPointConstant.Education.EducationEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEducation(int id, [FromBody] EducationInfo request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { "Invalid education data." }
                });
            }

            try
            {
                var isUpdated = await _educationService.UpdateEducationAsync(id, request);
                if (isUpdated)
                {
                    return Ok(new
                    {
                        status = "success",
                        message = "Update successful"
                    });
                }

                return NotFound(new { status = "error", message = "Education record not found." });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // DELETE api/education/{id}
        [HttpDelete(ApiEndPointConstant.Education.EducationEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            try
            {
                var isDeleted = await _educationService.DeleteEducationAsync(id);
                if (isDeleted)
                {
                    return Ok(new { status = "success", message = "Delete successful" });
                }

                return NotFound(new { status = "error", message = "Education record not found." });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // GET api/education
        [HttpGet(ApiEndPointConstant.Education.EducationsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllEducation([FromQuery] EducationFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var result = await _educationService.ViewAllEducationAsync(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
        }

        // GET api/education/users/{userId}/statistics
        [HttpGet(ApiEndPointConstant.Education.EducationStatsByUserEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEducationStatsByUser(int userId, [FromQuery] string groupBy = "SchoolName")
        {
            try
            {
                var stats = await _educationService.GetEducationStatsByUser(userId, groupBy);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = stats
                });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Bad request",
                    errors = new[] { ex.Message }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Internal server error"
                });
            }
        }

    }
}
