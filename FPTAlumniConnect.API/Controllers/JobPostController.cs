using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FPTAlumniConnect.API.Services.Implements;

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
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetJobPostById(int id)
        {
            try
            {
                var response = await _jobPostService.GetJobPostById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (BadHttpRequestException badEx)
            {
                _logger.LogError(badEx, "Bad request");
                return BadRequest(new { status = "error", message = badEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch job post");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPost(ApiEndPointConstant.JobPost.JobPostsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewJobPost([FromBody] JobPostInfo request)
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
                var id = await _jobPostService.CreateNewJobPost(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (BadHttpRequestException badEx)
            {
                _logger.LogError(badEx, "Bad request");
                return BadRequest(new { status = "error", message = badEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create job post");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpPatch(ApiEndPointConstant.JobPost.JobPostEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateJobPostInfo(int id, [FromBody] JobPostInfo request)
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
                var isSuccessful = await _jobPostService.UpdateJobPostInfo(id, request);
                if (!isSuccessful)
                {
                    return Ok(new { status = "error", message = "Update failed" });
                }

                return Ok(new { status = "success", message = "Update successful" });
            }
            catch (BadHttpRequestException badEx)
            {
                _logger.LogError(badEx, "Bad request");
                return BadRequest(new { status = "error", message = badEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update job post");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.JobPost.JobPostsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllJobPosts([FromQuery] JobPostFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _jobPostService.ViewAllJobPosts(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (BadHttpRequestException badEx)
            {
                _logger.LogError(badEx, "Bad request");
                return BadRequest(new { status = "error", message = badEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch job post");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        [HttpGet(ApiEndPointConstant.JobPost.CountEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CountAllJobPosts()
        {
            try
            {
                var response = await _jobPostService.CountAllJobPosts();
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch job post");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }


        [HttpGet(ApiEndPointConstant.JobPost.SearchJobPostsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchJobPosts(
    [FromQuery] string keyword,
    [FromQuery] int? minSalary = null,
    [FromQuery] int? maxSalary = null)
        {
            try
            {
                _logger.LogInformation("Searching job posts with keyword: {Keyword}, MinSalary: {MinSalary}, MaxSalary: {MaxSalary}",
                    keyword, minSalary, maxSalary);

                var jobPosts = await _jobPostService.SearchJobPosts(keyword, minSalary, maxSalary);

                if (jobPosts == null || !jobPosts.Any())
                {
                    return Ok(new
                    {
                        status = "success",
                        message = "No job posts found matching the criteria.",
                        data = new List<JobPostResponse>() // Return an empty list if no job posts found
                    });
                }

                return Ok(new
                {
                    status = "success",
                    message = "Search successful",
                    data = jobPosts
                });
            }
            catch (BadHttpRequestException badEx)
            {
                _logger.LogError(badEx, "Bad request");
                return BadRequest(new { status = "error", message = badEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search job posts");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

    }
}