using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.WorkExperience;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkExperienceController : BaseController<WorkExperienceController>
    {
        private readonly IWorkExperienceService _workExperienceService;
        private readonly IMapper _mapper;

        public WorkExperienceController(
            ILogger<WorkExperienceController> logger,
            IWorkExperienceService workExperienceService,
            IMapper mapper)
            : base(logger)
        {
            _workExperienceService = workExperienceService;
            _mapper = mapper;
        }


        [HttpPost(ApiEndPointConstant.WorkExperience.WorkExperiencesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWorkExperience([FromBody] WorkExperienceInfo request)
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
                var id = await _workExperienceService.CreateWorkExperienceAsync(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create work experience");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }


        [HttpGet(ApiEndPointConstant.WorkExperience.WorkExperienceEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWorkExperienceById(int id)
        {
            try
            {
                var response = await _workExperienceService.GetWorkExperienceByIdAsync(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch work experience");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }


        [HttpPut(ApiEndPointConstant.WorkExperience.WorkExperienceEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateWorkExperience(int id, [FromBody] WorkExperienceInfo request)
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
                var isSuccessful = await _workExperienceService.UpdateWorkExperienceAsync(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update work experience");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.WorkExperience.WorkExperiencesEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllWorkExperiences([FromQuery] WorkExperienceFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _workExperienceService.ViewAllWorkExperiencesAsync(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch work experience");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}