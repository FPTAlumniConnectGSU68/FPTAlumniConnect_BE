using Microsoft.AspNetCore.Mvc;
using StringUpdateApi.Services;

namespace StringUpdateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    private readonly VersionService _versionService;

    public VersionController(VersionService versionService)
    {
        _versionService = versionService;
    }

    /// <summary>
    /// Gets the current application version from configuration.
    /// </summary>
    /// <returns>A string representing the application version</returns>
    /// <response code="200">Returns the version string</response>
    [HttpGet("get-version")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult GetVersion()
    {
        var version = _versionService.GetVersion();
        return Ok(version);
    }

    /// <summary>
    /// Updates the application version to the current date (YYYY.MM.DD).
    /// </summary>
    /// <returns>A string confirming the new version</returns>
    /// <response code="200">Returns the new version string</response>
    /// <response code="500">If the version update fails</response>
    [HttpPost("update-version")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateVersion()
    {
        try
        {
            var newVersion = _versionService.UpdateVersionToCurrentDate();
            return Ok(newVersion);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to update version: {ex.Message}");
        }
    }
}