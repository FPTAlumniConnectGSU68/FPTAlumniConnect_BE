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

        // Lấy danh sách đơn ứng tuyển theo JobPostId
        [HttpGet(ApiEndPointConstant.JobApplication.JobApplicationJPIdEndPoint)]
        [ProducesResponseType(typeof(List<JobApplicationResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplicationsByJobPostId(int jobPostId)
        {
            var response = await _jobApplicationService.GetJobApplicationsByJobPostId(jobPostId);
            return Ok(response);
        }

        // Lấy danh sách đơn ứng tuyển theo CV
        [HttpGet(ApiEndPointConstant.JobApplication.JobApplicationCVIdEndPoint)]
        [ProducesResponseType(typeof(List<JobApplicationResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetApplicationsByCvId(int cvId)
        {
            var response = await _jobApplicationService.GetJobApplicationsByCvId(cvId);
            return Ok(response);
        }

        // Xoá đơn ứng tuyển
        //[HttpDelete(ApiEndPointConstant.JobApplication.JobApplicationEndPoint)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        //public async Task<IActionResult> DeleteJobApplication(int id)
        //{
        //    var success = await _jobApplicationService.DeleteJobApplication(id);
        //    return Ok(success ? "DeleteSuccess" : "DeleteFailed");
        //}

        // Kiểm tra đã nộp đơn hay chưa
        [HttpGet(ApiEndPointConstant.JobApplication.JobApplicationCheckAppliedEndPoint)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> HasAlreadyApplied([FromQuery] int jobPostId, [FromQuery] int cvId)
        {
            var result = await _jobApplicationService.HasAlreadyApplied(jobPostId, cvId);
            return Ok(result);
        }

        // Đếm tổng số đơn ứng tuyển
        [HttpGet(ApiEndPointConstant.JobApplication.JobApplicationCountEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> CountAllApplications()
        {
            var count = await _jobApplicationService.CountAllJobApplications();
            return Ok(count);
        }
    }
}
