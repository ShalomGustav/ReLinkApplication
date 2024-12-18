using Microsoft.EntityFrameworkCore;
using ReLinkApplication.Models;
using ReLinkApplication.Repositories;

namespace ReLinkApplication.Services;

public class UrlService
{
    private readonly UrlDbContext _dBContext;
    private readonly string _defaultUrl = "https://relink.ms/";
    private readonly HashSet<string> _hashShortUrl;
    private readonly HashSet<string> _hashLongUrl;
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public UrlService(UrlDbContext context)
    {
        _dBContext = context;
        _hashShortUrl = new HashSet<string>(_dBContext.Url.Select(x => x.ShortUrl).ToList());
        _hashLongUrl = new HashSet<string>(_dBContext.Url.Select(x => x.LongUrl).ToList());
    }

    public async Task<List<Url>> GetAllUrlAsync()
    {
        var result = await _dBContext.Url.ToListAsync();

        return result;
    }

    public async Task<Url> GetUrlById(Guid id)
    {
        if(id == Guid.Empty)
        {
            throw new ArgumentNullException("Guid can not be null or empty.");
        }

        var url = await _dBContext.Url.FirstOrDefaultAsync(x => x.Id == id);

        return url;
    }

    public async Task<Url?> GetByLongUrlAsync(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            throw new ArgumentNullException(nameof(longUrl), "The URL cannot be null or empty.");
        }

        var result = await _dBContext.Url.FirstOrDefaultAsync(x => x.LongUrl == longUrl);

        return result;
    }

    public async Task<Url> CreateByLongUrlAsync(string longUrl)
    {
        if (string.IsNullOrEmpty(longUrl))
        {
            throw new ArgumentNullException("Url not can be is null. ");
        }

        var existLongUrl = await GetByLongUrlAsync(longUrl);

        if(existLongUrl != null)
        {
            return existLongUrl;
        }

        var newUrl = new Url
        {
            LongUrl = longUrl,
            ShortUrl = await CreateUniqueShortUrlAsync()
        };

        await _dBContext.AddAsync(newUrl);
        await _dBContext.SaveChangesAsync();

        _hashLongUrl.Add(newUrl.LongUrl);
        _hashShortUrl.Add(newUrl.ShortUrl);

        return newUrl;
    }

    public async Task<Url> UpdateByLongUrlAsync(string longUrl)
    {
        if (string.IsNullOrEmpty(longUrl))
        {
            throw new ArgumentNullException("Url not can be is null. ");
        }

        var existLongUrl = await GetByLongUrlAsync(longUrl);

        if (existLongUrl == null)
        {
            throw new ArgumentNullException("Url is not exist");
        }

        var newUrl = new Url
        {
            LongUrl = longUrl,
            ShortUrl = await CreateUniqueShortUrlAsync()
        };

        await _dBContext.AddAsync(newUrl);
        await _dBContext.SaveChangesAsync();

        _hashLongUrl.Add(newUrl.LongUrl);
        _hashShortUrl.Add(newUrl.ShortUrl);

        return newUrl;
    }

    public Task<bool>

    private async Task<string> CreateUniqueShortUrlAsync()
    {
        var random = new Random();
        string shortUrl;

        do
        {
            shortUrl = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            shortUrl = _defaultUrl + shortUrl;

        } while (await _dBContext.Url.AnyAsync(x => x.ShortUrl == shortUrl));
        //_hashLongUrl.Contains(shortUrl)

        return shortUrl;
    }
}
