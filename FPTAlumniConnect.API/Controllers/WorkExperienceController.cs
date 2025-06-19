using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.WorkExperience;
using FPTAlumniConnect.DataTier.Paginate;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWorkExperience([FromBody] WorkExperienceInfo request)
        {
            try
            {
                int workExperienceId = await _workExperienceService.CreateWorkExperienceAsync(request);
                return CreatedAtAction(nameof(GetWorkExperienceById), new { id = workExperienceId }, workExperienceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating work experience.");
                return BadRequest("An error occurred while creating the work experience.");
            }
        }


        [HttpGet(ApiEndPointConstant.WorkExperience.WorkExperienceEndPoint)]
        [ProducesResponseType(typeof(WorkExperienceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWorkExperienceById(int id)
        {
            try
            {
                WorkExperienceResponse workExperience = await _workExperienceService.GetWorkExperienceByIdAsync(id);
                return Ok(workExperience);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Work experience not found.");
                return NotFound("Work experience not found.");
            }
        }


        [HttpPut(ApiEndPointConstant.WorkExperience.WorkExperienceEndPoint)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateWorkExperience(int id, [FromBody] WorkExperienceInfo request)
        {
            try
            {
                bool isUpdated = await _workExperienceService.UpdateWorkExperienceAsync(id, request);
                if (isUpdated)
                {
                    return Ok("Work experience updated successfully.");
                }
                else
                {
                    return NotFound("Work experience not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating work experience.");
                return BadRequest("An error occurred while updating the work experience.");
            }
        }

        [HttpDelete(ApiEndPointConstant.WorkExperience.WorkExperienceEndPoint)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWorkExperience(int id)
        {
            try
            {
                bool isDeleted = await _workExperienceService.DeleteWorkExperienceAsync(id);
                if (isDeleted)
                {
                    return Ok("Work experience deleted successfully.");
                }
                else
                {
                    return NotFound("Work experience not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting work experience.");
                return BadRequest("An error occurred while deleting the work experience.");
            }
        }

        [HttpGet(ApiEndPointConstant.WorkExperience.WorkExperiencesEndPoint)]
        [ProducesResponseType(typeof(IPaginate<WorkExperienceResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllWorkExperiences([FromQuery] WorkExperienceFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                IPaginate<WorkExperienceResponse> workExperiences = await _workExperienceService.ViewAllWorkExperiencesAsync(filter, pagingModel);
                return Ok(workExperiences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching work experiences.");
                return BadRequest("An error occurred while fetching the work experiences.");
            }
        }
    }
}