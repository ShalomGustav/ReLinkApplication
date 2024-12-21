using Microsoft.AspNetCore.Mvc;
using ReLinkApplication.Services;

namespace ReLinkApplication.Controllers;

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
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            throw new ArgumentNullException(nameof(longUrl));
        }

        var shortUrl = await _urlService.CreateShortUrlAsync(longUrl);

        return Ok(new {shortUrl});
    }

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> RedirectToLongUrlAsync(string shortUrl)
    {
        var longUrl = await _urlService.GetLongUrlByShortUrlAsync(Uri.UnescapeDataString(shortUrl));

        return RedirectToRoute(longUrl);
    }
}
