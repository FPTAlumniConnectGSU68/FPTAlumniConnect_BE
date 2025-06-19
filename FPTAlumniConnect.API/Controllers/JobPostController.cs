using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using Microsoft.AspNetCore.Mvc;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class JobPostController : BaseController<JobPostController>
    {
        private readonly IJobPostService _jobPostService;

        public JobPostController(ILogger<JobPostController> logger, IJobPostService jobPostService)
            : base(logger)
        {
            _jobPostService = jobPostService;
        }

        [HttpGet(ApiEndPointConstant.JobPost.JobPostEndPoint)]
        [ProducesResponseType(typeof(JobPostResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetJobPostById(int id)
        {
            var response = await _jobPostService.GetJobPostById(id);
            return Ok(response);
        }

        [HttpPost(ApiEndPointConstant.JobPost.JobPostsEndPoint)]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateNewJobPost([FromBody] JobPostInfo request)
        {
            var id = await _jobPostService.CreateNewJobPost(request);
            return CreatedAtAction(nameof(GetJobPostById), new { id }, id);
        }

        [HttpPatch(ApiEndPointConstant.JobPost.JobPostEndPoint)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateJobPostInfo(int id, [FromBody] JobPostInfo request)
        {
            var isSuccessful = await _jobPostService.UpdateJobPostInfo(id, request);
            if (!isSuccessful) return Ok("UpdateStatusFailed");
            return Ok("UpdateStatusSuccess");
        }

        [HttpGet(ApiEndPointConstant.JobPost.JobPostsEndPoint)]
        [ProducesResponseType(typeof(JobPostResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ViewAllJobPosts([FromQuery] JobPostFilter filter, [FromQuery] PagingModel pagingModel)
        {
            var response = await _jobPostService.ViewAllJobPosts(filter, pagingModel);
            return Ok(response);
        }
    }
}
