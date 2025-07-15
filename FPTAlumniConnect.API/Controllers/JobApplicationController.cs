using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobApplication;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class JobApplicationController : BaseController<JobApplicationController>
    {
        private readonly IJobApplicationService _jobApplicationService;

        public JobApplicationController(ILogger<JobApplicationController> logger, IJobApplicationService jobApplicationService)
            : base(logger)
        {
            _jobApplicationService = jobApplicationService;
        }

        [HttpGet(ApiEndPointConstant.JobApplication.JobApplicationEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetJobApplicationById(int id)
        {
            try
            {
                var response = await _jobApplicationService.GetJobApplicationById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch job application");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.JobApplication.JobApplicationsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateNewJobApplication([FromBody] JobApplicationInfo request)
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
                var id = await _jobApplicationService.CreateNewJobApplication(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create job application");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.JobApplication.JobApplicationEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateJobApplicationInfo(int id, [FromBody] JobApplicationInfo request)
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
                var isSuccessful = await _jobApplicationService.UpdateJobApplicationInfo(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update job application");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.JobApplication.JobApplicationsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllJobApplications([FromQuery] JobApplicationFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _jobApplicationService.ViewAllJobApplications(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch job application");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Lấy danh sách đơn ứng tuyển theo JobPostId
        [HttpGet(ApiEndPointConstant.JobApplication.JobApplicationJPIdEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetApplicationsByJobPostId(int jobPostId)
        {
            try
            {
                var response = await _jobApplicationService.GetJobApplicationsByJobPostId(jobPostId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch job application");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Lấy danh sách đơn ứng tuyển theo CV
        [HttpGet(ApiEndPointConstant.JobApplication.JobApplicationCVIdEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetApplicationsByCvId(int cvId)
        {
            try
            {
                var response = await _jobApplicationService.GetJobApplicationsByCvId(cvId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch job application");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Kiểm tra đã nộp đơn hay chưa
        [HttpGet(ApiEndPointConstant.JobApplication.JobApplicationCheckAppliedEndPoint)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> HasAlreadyApplied([FromQuery] int jobPostId, [FromQuery] int cvId)
        {
            try
            {
                var response = await _jobApplicationService.HasAlreadyApplied(jobPostId, cvId);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch job application");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        // Đếm tổng số đơn ứng tuyển
        [HttpGet(ApiEndPointConstant.JobApplication.JobApplicationCountEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(int), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CountAllApplications()
        {
            try
            {
                var response = await _jobApplicationService.CountAllJobApplications();
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch job application");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}
