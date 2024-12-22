using Microsoft.AspNetCore.Mvc;
using ReLinkApplication.Services;

namespace ReLinkApplication.Controllers;

[ApiController]
public class UrlControllers : ControllerBase
{
    private readonly UrlService _urlService;
    private readonly string _baseUrl;
    private const string _protocol = "http://";

    public UrlControllers(UrlService urlService, IConfiguration configuration)
    {
        _urlService = urlService;
        _baseUrl = configuration["ApplicationSettings:BaseUrl"];
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateShortUrlAsync([FromBody] string longUrl)
    {
        var code = await _urlService.CreateShortUrlAsync(longUrl);

        if (string.IsNullOrEmpty(code))
        {
            throw new NullReferenceException("URL not exist");
        }

        var shortUrl = _baseUrl + code;

        return Ok(new { shortUrl });
    }

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> RedirectToLongUrlAsync(string shortUrl)
    {
        var longUrl = await _urlService.GetLongUrlByShortUrlAsync(Uri.UnescapeDataString(shortUrl));

        if (!Uri.IsWellFormedUriString(longUrl, UriKind.Absolute))
        {
            longUrl = _protocol + longUrl;
        }

        return Redirect(longUrl);
    }

    
}
