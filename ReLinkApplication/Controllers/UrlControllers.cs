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

        return Ok(shortUrl);
    }

    [HttpGet("redirect")]
    public async Task<IActionResult> RedirectToLongUrlAsync([FromQuery] string shortUrl)
    {
        var decodedShortUrl = Uri.UnescapeDataString(shortUrl);

        var longUrl = await _urlService.GetLongUrlByShortUrlAsync(decodedShortUrl);
        return Redirect(longUrl);
    }
}
