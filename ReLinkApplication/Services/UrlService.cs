using Microsoft.EntityFrameworkCore;
using ReLinkApplication.Models;
using ReLinkApplication.Repositories;

namespace ReLinkApplication.Services;

public class UrlService
{
    private readonly UrlDbContext _dbContext;
    private readonly string _defaultUrl = "https://relink.ms/";
    private readonly HashSet<string> _shortUrlCache;
    private readonly HashSet<string> _longUrlCache;
    private readonly Random _randomGenerator = new Random();
    const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public UrlService(UrlDbContext context)
    {
        _dbContext = context;
        _shortUrlCache = new HashSet<string>(_dbContext.Url.Select(x => x.ShortUrl).ToList());
        _longUrlCache = new HashSet<string>(_dbContext.Url.Select(x => x.LongUrl).ToList());
    }

    public async Task<List<Url>> GetAllUrlAsync()
    {
        return await _dbContext.Url.ToListAsync();
    }

    public async Task<Url?> GetUrlById(Guid id)
    {
        if(id == Guid.Empty)
        {
            throw new ArgumentNullException("ID cannot be null or empty.");
        }

        return await _dbContext.Url.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Url> GetByLongUrlAsync(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            throw new ArgumentNullException(nameof(longUrl), "The URL cannot be null or empty.");
        }

        var result = await _dbContext.Url.FirstOrDefaultAsync(x => x.LongUrl == longUrl);

        if(result == null)
        {
            throw new ArgumentNullException("The specified URL does not exist.");
        }

        return result;
    }

    public async Task<Url> CreateUpdateByLongUrlAsync(string longUrl)
    {
        if (string.IsNullOrEmpty(longUrl))
        {
            throw new ArgumentNullException("URL cannot be null or empty.");
        }

        var existingUrl = await GetByLongUrlAsync(longUrl);

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

        UpdateHashSet(newUrl);

        return newUrl;
    }

    public async Task<bool> DeleteByLongUrlAsync(string longUrl)
    {
        if (string.IsNullOrEmpty(longUrl))
        {
            throw new ArgumentNullException("URL cannot be null or empty.");
        }

        var existingUrl = await GetByLongUrlAsync(longUrl);

        if (existingUrl == null)
        {
            return false;
        }

        _dbContext.Url.Remove(existingUrl);
        await _dbContext.SaveChangesAsync();

        DeleteFromHashSet(existingUrl);

        return true;
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

    private void UpdateHashSet(Url url)
    {
        _longUrlCache.Add(url.LongUrl);
        _shortUrlCache.Add(url.ShortUrl);
    }

    private void DeleteFromHashSet(Url url)
    {
        _longUrlCache.Remove(url.LongUrl);
        _shortUrlCache.Remove(url.ShortUrl);
    }
}
