using Microsoft.AspNetCore.Mvc;
using ReLinkApplication.Services;
using System.Text.RegularExpressions;

namespace ReLinkApplication.Controllers;

[ApiController]
public class UrlControllers : ControllerBase
{
    private readonly UrlServices _urlService;
    private readonly string _baseUrl;
    private const string _protocol = "http://";

    public UrlControllers(UrlServices urlService, IConfiguration configuration)
    {
        _urlService = urlService;
        _baseUrl = configuration["ApplicationSettings:BaseUrl"];
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateShortUrlAsync([FromBody] string longUrl)
    {
        if (!IsValidUrl(longUrl,false))
        {
            return BadRequest("Invalid long URL format.");
        }

        var code = await _urlService.CreateShortUrlAsync(longUrl);

        if (string.IsNullOrEmpty(code))
        {
            throw new InvalidOperationException("The short link could not be generated.");
        }

        var shortUrl = _baseUrl + code;

        return Ok(new { shortUrl });
    }

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> RedirectToLongUrlAsync(string shortUrl)
    {
        if (!IsValidUrl(shortUrl, true))
        {
            return BadRequest("Invalid long URL format.");
        }

        var longUrl = await _urlService.GetLongUrlByShortUrlAsync(Uri.UnescapeDataString(shortUrl));

        if (!Uri.IsWellFormedUriString(longUrl, UriKind.Absolute))
        {
            longUrl = _protocol + longUrl;
        }

        return Redirect(longUrl);
    }

    private bool IsValidUrl(string longUrl, bool isShortUrl)
    {
        if (string.IsNullOrEmpty(longUrl))
        {
            return false;
        }

        var patternByLongUrl = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
        var shortUrlPattern = @"^[a-zA-Z]{6}$";

        var pattern = isShortUrl ? shortUrlPattern : patternByLongUrl;
        var result = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        return result.IsMatch(pattern);
    }
}
