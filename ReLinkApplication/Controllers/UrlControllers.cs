using Microsoft.AspNetCore.Mvc;
using ReLinkApplication.Services;

namespace ReLinkApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UrlControllers : ControllerBase
{
    private readonly UrlService _urlService;

    public UrlControllers(UrlService urlService)
    {
        _urlService = urlService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateShortUrlAsync([FromBody] string longUrl)
    {
        var shortUrl = await _urlService.CreateShortUrlAsync(longUrl);

        return Ok(new {shortUrl});
    }

    [HttpGet("redirect")]
    public async Task<IActionResult> RedirectToLongUrlAsync([FromQuery] string shortUrl)
    {
        var decodedShortUrl = Uri.UnescapeDataString(shortUrl);

        try
        {
            var longUrl = await _urlService.GetLongUrlByShortUrlAsync(decodedShortUrl);
            return Redirect(longUrl);
        }
        catch(KeyNotFoundException)
        {
            return NotFound("Short URL not found");
        }
    }
}
