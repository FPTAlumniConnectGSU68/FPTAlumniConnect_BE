using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Constants;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using Microsoft.AspNetCore.Mvc;

namespace FPTAlumniConnect.API.Controllers
{
    [ApiController]
    public class CVController : BaseController<CVController>
    {
        private readonly ICVService _cVService;

        public CVController(ILogger<CVController> logger, ICVService cVService) : base(logger)
        {
            _cVService = cVService;
        }

        /// <summary>
        /// Retrieves a CV by its ID, including associated skills.
        /// </summary>
        /// <param name="id">The ID of the CV to retrieve.</param>
        /// <returns>A CV object with skill IDs if found; otherwise, an error response.</returns>
        [HttpGet(ApiEndPointConstant.CV.CVEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCVById(int id)
        {
            try
            {
                var response = await _cVService.GetCVById(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch CV with ID: {Id}", id);
                return BadRequest(new { status = "error", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch CV with ID: {Id}", id);
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        /// <summary>
        /// Retrieves a CV by its associated user ID, including associated skills.
        /// </summary>
        /// <param name="id">The ID of the user whose CV is to be retrieved.</param>
        /// <returns>A CV object with skill IDs if found; otherwise, an error response.</returns>
        [HttpGet(ApiEndPointConstant.CV.CVUserEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCVByUserId(int id)
        {
            try
            {
                var response = await _cVService.GetCVByUserId(id);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch CV for user ID: {Id}", id);
                return BadRequest(new { status = "error", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch CV for user ID: {Id}", id);
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        /// <summary>
        /// Creates a new CV with associated skills.
        /// </summary>
        /// <param name="request">The CV information, including skill IDs, to create.</param>
        /// <returns>The ID of the newly created CV.</returns>
        [HttpPost(ApiEndPointConstant.CV.CVsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateNewCV([FromBody] CVInfo request)
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
                // Validate SkillIds if provided
                if (request.SkillIds != null && request.SkillIds.Any(id => id <= 0))
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Invalid skill IDs provided"
                    });
                }

                var id = await _cVService.CreateNewCV(request);
                return StatusCode(201, new
                {
                    status = "success",
                    message = "Resource created successfully",
                    data = new { id }
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to create CV");
                return BadRequest(new { status = "error", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create CV");
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        /// <summary>
        /// Retrieves a paginated list of CVs based on the provided filter.
        /// </summary>
        /// <param name="filter">The filter criteria for CVs.</param>
        /// <param name="pagingModel">The pagination parameters.</param>
        /// <returns>A paginated list of CVs.</returns>
        [HttpGet(ApiEndPointConstant.CV.CVsEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewAllCV([FromQuery] CVFilter filter, [FromQuery] PagingModel pagingModel)
        {
            try
            {
                var response = await _cVService.ViewAllCV(filter, pagingModel);
                return Ok(new
                {
                    status = "success",
                    message = "Request successful",
                    data = response
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch CVs with filter: {@Filter}", filter);
                return BadRequest(new { status = "error", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch CVs with filter: {@Filter}", filter);
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        /// <summary>
        /// Updates an existing CV, including its associated skills.
        /// </summary>
        /// <param name="id">The ID of the CV to update.</param>
        /// <param name="request">The updated CV information, including skill IDs.</param>
        /// <returns>Confirmation of the update operation.</returns>
        [HttpPatch(ApiEndPointConstant.CV.CVEndPoint)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCVInfo(int id, [FromBody] CVInfo request)
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
                // Validate SkillIds if provided
                if (request.SkillIds != null && request.SkillIds.Any(id => id <= 0))
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Invalid skill IDs provided"
                    });
                }

                var isSuccessful = await _cVService.UpdateCVInfo(id, request);
                if (!isSuccessful)
                {
                    return Ok(new
                    {
                        status = "error",
                        message = "Update failed",
                        data = new { id }
                    });
                }

                return Ok(new
                {
                    status = "success",
                    message = "Update successful",
                    data = new { id }
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to update CV with ID: {Id}", id);
                return BadRequest(new { status = "error", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update CV with ID: {Id}", id);
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        /// <summary>
        /// Shares a CV via email.
        /// </summary>
        /// <param name="request">The details for sharing the CV, including recipient email and message.</param>
        /// <returns>Confirmation of the share operation.</returns>
        //[HttpPost("share")]
        //[ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> ShareCvByEmail([FromBody] ShareCvRequest request)
        //{
        //    if (request == null)
        //    {
        //        return BadRequest(new
        //        {
        //            status = "error",
        //            message = "Bad request",
        //            errors = new[] { "Request body is null or malformed" }
        //        });
        //    }
        //    try
        //    {
        //        await _cVService.ShareCvByEmailAsync(request);
        //        return Ok(new
        //        {
        //            status = "success",
        //            message = "CV shared successfully",
        //            data = new { cvId = request.CvId }
        //        });
        //    }
        //    catch (BadHttpRequestException ex)
        //    {
        //        _logger.LogError(ex, "Failed to share CV with ID: {CvId}", request.CvId);
        //        return BadRequest(new { status = "error", message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to share CV with ID: {CvId}", request.CvId);
        //        return StatusCode(500, new { status = "error", message = "Internal server error" });
        //    }
        //}

        /// <summary>
        /// Toggles the job-looking status of a CV.
        /// </summary>
        /// <param name="cvId">The ID of the CV to toggle.</param>
        /// <returns>The updated job-looking status.</returns>
        [HttpPatch("{cvId}/toggle-job-looking")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ToggleJobLooking(int cvId)
        {
            try
            {
                var result = await _cVService.ToggleIsLookingForJobAsync(cvId);
                return Ok(new
                {
                    status = "success",
                    message = "Job looking status toggled successfully",
                    data = new { cvId, isLooking = result }
                });
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to toggle job looking status for CV ID: {CvId}", cvId);
                return BadRequest(new { status = "error", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to toggle job looking status for CV ID: {CvId}", cvId);
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }

        /// <summary>
        /// Exports a CV to PDF format.
        /// </summary>
        /// <param name="cvId">The ID of the CV to export.</param>
        /// <returns>A PDF file containing the CV data.</returns>
        [HttpGet("{cvId}/export")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportCvToPdf(int cvId)
        {
            try
            {
                var pdfBytes = await _cVService.ExportCvToPdfAsync(cvId);
                if (pdfBytes == null)
                {
                    return BadRequest(new { status = "error", message = "Failed to generate PDF" });
                }
                return File(pdfBytes, "application/pdf", $"cv_{cvId}.pdf");
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to export CV with ID: {CvId}", cvId);
                return BadRequest(new { status = "error", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export CV with ID: {CvId}", cvId);
                return StatusCode(500, new { status = "error", message = "Internal server error" });
            }
        }
    }
}