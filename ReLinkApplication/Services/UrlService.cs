using Microsoft.EntityFrameworkCore;
using ReLinkApplication.Models;
using ReLinkApplication.Repositories;

namespace ReLinkApplication.Services;

public class UrlService
{
    private readonly UrlDbContext _dbContext;
    const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public UrlService(UrlDbContext context)
    {
        _dbContext = context;
    }

    public async Task<string> GetLongUrlByShortUrlAsync(string shortUrl)
    {
        if (string.IsNullOrWhiteSpace(shortUrl))
        {
            throw new ArgumentNullException(nameof(shortUrl), "Short URL cannot be null or empty.");
        }

        var url = await _dbContext.Url.FirstOrDefaultAsync(x => x.ShortUrl == shortUrl);

        if (url == null)
        {
            throw new KeyNotFoundException("Short URL not found.");
        }

        return url.LongUrl;
    }

    public async Task<string> CreateShortUrlAsync(string longUrl)
    {
        if (string.IsNullOrEmpty(longUrl))
        {
            throw new ArgumentNullException("URL cannot be null or empty.");
        }

        var existingUrl = await _dbContext.Url.FirstOrDefaultAsync(x => x.LongUrl == longUrl);

        if (existingUrl != null)
        {
            return existingUrl.ShortUrl;
        }

        var shortUrl = await CreateUniqueShortUrlAsync();

        var url = new Url
        {
            LongUrl = longUrl,
            ShortUrl = shortUrl
        };

        _dbContext.Url.Add(url);
        await _dbContext.SaveChangesAsync();

        return shortUrl;
    }

    private async Task<string> CreateUniqueShortUrlAsync()
    {
        var shortCode = new string(Enumerable.Range(0, 6)
            .Select(x => AllowedChars[new Random().Next(AllowedChars.Length)]).ToArray());

        var exist = await _dbContext.Url.AnyAsync(x => x.ShortUrl == shortCode);
        
        if (!exist)
        {
            return shortCode;
        }

        return await CreateUniqueShortUrlAsync();
    }
}
