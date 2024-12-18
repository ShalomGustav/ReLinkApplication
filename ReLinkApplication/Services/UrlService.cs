﻿using Microsoft.EntityFrameworkCore;
using ReLinkApplication.Models;
using ReLinkApplication.Repositories;

namespace ReLinkApplication.Services;

public class UrlService
{
    private readonly UrlDbContext _dbContext;
    private readonly string _baseUrl;
    const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public UrlService(UrlDbContext context, IConfiguration configuration)
    {
        _dbContext = context;
        _baseUrl = configuration["ApplicationSettings:BaseUrl"];
    }

    public async Task<string> CreateShortUrlAsync(string longUrl)
    {
        if(string.IsNullOrEmpty(longUrl))
        {
            throw new ArgumentNullException("URL cannot be null or empty.");
        }

        var existingUrl = await _dbContext.Url.FirstOrDefaultAsync(x => x.LongUrl == longUrl);

        if(existingUrl != null)
        {
            return existingUrl.ShortUrl;
        }

        var shortCode = await CreateUniqueShortUrlAsync();

        var url = new Url
        {
            LongUrl = longUrl,
            ShortUrl = $"{_baseUrl}/{shortCode}"
        };

        await _dbContext.AddAsync(url);
        await _dbContext.SaveChangesAsync();

        return url.ShortUrl;
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

    private async Task<string> CreateUniqueShortUrlAsync()
    {
        var shortUrl = new string(Enumerable.Range(0, 6)
            .Select(_ => AllowedChars[new Random().Next(AllowedChars.Length)]).ToArray());

        var exist = await _dbContext.Url.AnyAsync(x => x.ShortUrl == _baseUrl + shortUrl);
        
        if (!exist)
        {
            return shortUrl;
        }

        return await CreateUniqueShortUrlAsync();
    }
}
