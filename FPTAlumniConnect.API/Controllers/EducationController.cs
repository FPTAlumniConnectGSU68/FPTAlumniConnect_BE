using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Education;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.DataTier.Paginate;

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
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEducation([FromBody] EducationInfo request)
        {
            if (request == null)
            {
                return BadRequest("Invalid education data.");
            }

            try
            {
                int educationId = await _educationService.CreateEducationAsync(request);
                return CreatedAtAction(nameof(GetEducationById), new { id = educationId }, null);
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/education/{id}
        [HttpGet(ApiEndPointConstant.Education.EducationEndPoint)] // Specify route here
        [ProducesResponseType(typeof(EducationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEducationById(int id)
        {
            try
            {
                EducationResponse response = await _educationService.GetEducationByIdAsync(id);
                return Ok(response);
            }
            catch (BadHttpRequestException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT api/education/{id}
        [HttpPut(ApiEndPointConstant.Education.EducationEndPoint)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateEducation(int id, [FromBody] EducationInfo request)
        {
            if (request == null)
            {
                return BadRequest("Invalid education data.");
            }

            try
            {
                bool isUpdated = await _educationService.UpdateEducationAsync(id, request);
                if (isUpdated)
                {
                    return NoContent();
                }
                return NotFound("Education record not found.");
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/education/{id}
        [HttpDelete(ApiEndPointConstant.Education.EducationEndPoint)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            try
            {
                bool isDeleted = await _educationService.DeleteEducationAsync(id);
                if (isDeleted)
                {
                    return NoContent();
                }
                return NotFound("Education record not found.");
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/education
        [HttpGet(ApiEndPointConstant.Education.EducationsEndPoint)]
        [ProducesResponseType(typeof(IPaginate<EducationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllEducation([FromQuery] EducationFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var result = await _educationService.ViewAllEducationAsync(filter, pagingModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching data: {ex.Message}");
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