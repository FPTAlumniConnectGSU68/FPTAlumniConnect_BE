using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.JobApplication;
using FPTAlumniConnect.DataTier.Paginate;

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
        [ProducesResponseType(typeof(JobApplicationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetJobApplicationById(int id)
        {
            var response = await _jobApplicationService.GetJobApplicationById(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.JobApplication.JobApplicationsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewJobApplication([FromBody] JobApplicationInfo request)
        {
            var id = await _jobApplicationService.CreateNewJobApplication(request);
            return CreatedAtAction(nameof(GetJobApplicationById), new { id }, id);
        }

        [HttpPatch(ApiEndPointConstant.JobApplication.JobApplicationEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateJobApplicationInfo(int id, [FromBody] JobApplicationInfo request)
        {
            var isSuccessful = await _jobApplicationService.UpdateJobApplicationInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [HttpGet(ApiEndPointConstant.JobApplication.JobApplicationsEndPoint)]
        [ProducesResponseType(typeof(JobApplicationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllJobApplications([FromQuery] JobApplicationFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _jobApplicationService.ViewAllJobApplications(filter, pagingModel);
            return Ok(response);
        }
    }
}
