using Microsoft.AspNetCore.Mvc;
using StringUpdateApi.Services;

namespace StringUpdateApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StringUpdateController : ControllerBase
{
    private readonly StringCounterService _counterService;

    public StringUpdateController(StringCounterService counterService)
    {
        _counterService = counterService;
    }

    /// <summary>
    /// Gets the next string value, incrementing with each call.
    /// </summary>
    /// <returns>A string in the format 'Update-{number}'</returns>
    /// <response code="200">Returns the updated string</response>
    [HttpGet("get-next-string")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult GetNextString()
    {
        var result = _counterService.GetNextString();
        return Ok(result);
    }
}