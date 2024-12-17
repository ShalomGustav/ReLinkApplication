using Microsoft.EntityFrameworkCore;
using ReLinkApplication.Models;
using ReLinkApplication.Repositories;

namespace ReLinkApplication.Services;

public class UrlService
{
    private readonly UrlDbContext _context;
    private readonly string _defaultUrl = "https://relink.ms/";

    public UrlService(UrlDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetShortUrlAsync(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            throw new ArgumentNullException(nameof(longUrl));
        }

        var existUrl = await _context.Url.AnyAsync(x => x.LongUrl == longUrl);

        if (!existUrl)
        {
            var newLoGenerateUniqueShortUrlAsync(longUrl);
        }


        var shortUrl = CreateShortUrl();

        var urlOnDb = await _context.Url.AnyAsync(x => x.ShortUrl == _defaultUrl + shortUrl);


        var mappingUrl = new Url
        {
            LongUrl = longUrl,
            ShortUrl = shortUrl
        };

        _context.Url.Add(mappingUrl);
        var result = await _context.SaveChangesAsync();

        return shortUrl;
    }

    private async Task<string> GenerateUniqueShortUrlAsync(string longUrl)
    {
        string shortUrl;
        var existUrl = await _context.Url.AnyAsync(x => x.LongUrl == longUrl);

        do
        {
            shortUrl = CreateShortUrl();

        }
        while (existUrl);

        return shortUrl;
    }

    private string CreateShortUrl()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        var random = new Random();

        var randomUrl = new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        return randomUrl;
    }
}
