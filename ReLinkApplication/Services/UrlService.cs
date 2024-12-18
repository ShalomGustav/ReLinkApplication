using Microsoft.EntityFrameworkCore;
using ReLinkApplication.Models;
using ReLinkApplication.Repositories;

namespace ReLinkApplication.Services;

public class UrlService
{
    private readonly UrlDbContext _dbContext;
    private readonly string _defaultUrl = "https://relink.ms/";
    private readonly Random _randomGenerator = new Random();
    const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public UrlService(UrlDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Url> GetByUrlAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentNullException("URL cannot be null or empty.");
        }

        var result = await _dbContext.Url.FirstOrDefaultAsync(x => x.LongUrl == url);

        if(result == null)
        {
            throw new ArgumentNullException("The specified URL does not exist.");
        }

        return result;
    }

    public async Task<Url> CreateShortUrlAsync(string longUrl)
    {
        var existingUrl = await GetByUrlAsync(longUrl);

        if(existingUrl != null)
        {
            return existingUrl;
        }

        var newUrl = new Url
        {
            LongUrl = longUrl,
            ShortUrl = await CreateUniqueShortUrlAsync()
        };

        await _dbContext.AddAsync(newUrl);
        await _dbContext.SaveChangesAsync();

        return newUrl;
    }

    public async Task<string> GetLongUrlByShortUrlAsync(string shortUrl)
    {
        if (string.IsNullOrWhiteSpace(shortUrl))
        {
            throw new ArgumentNullException(nameof(shortUrl), "Short URL cannot be null or empty.");
        }

        var url = await GetByUrlAsync(shortUrl);

        if (url == null)
        {
            throw new KeyNotFoundException("Short URL not found.");
        }

        return url.LongUrl;
    }

    private async Task<string> CreateUniqueShortUrlAsync()
    {
        var shortUrl = _defaultUrl + new string(Enumerable.Range(0, 6)
            .Select(_ => AllowedChars[_randomGenerator.Next(AllowedChars.Length)]).ToArray());

        var exist = await _dbContext.Url.AnyAsync(x => x.ShortUrl == shortUrl);
        
        if (!exist)
        {
            return shortUrl;
        }

        return await CreateUniqueShortUrlAsync();
    }
}
