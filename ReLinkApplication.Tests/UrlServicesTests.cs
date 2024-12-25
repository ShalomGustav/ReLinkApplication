using Moq;
using Moq.EntityFrameworkCore;
using ReLinkApplication.Models;
using ReLinkApplication.Repositories;
using ReLinkApplication.Services;

namespace ReLinkApplication.Tests;

public class UrlServicesTests
{
    private readonly Mock<UrlDbContext> _mockDbContext;
    private readonly UrlServices _urlServices;
    public UrlServicesTests()
    {
        _mockDbContext = new Mock<UrlDbContext>();
        _urlServices = new UrlServices(_mockDbContext.Object);
    }

    #region Tests
    [Fact]
    public async Task GetLongUrlByShortUrlAsyncTests()
    {
        // Arrange
        var url = GetUrl();
        var longUrl = url.First().LongUrl;
        var shortUrl = url.First().ShortUrl;

        _mockDbContext.Setup(x => x.Url).ReturnsDbSet(url);

        // Act
        var result = await _urlServices.GetLongUrlByShortUrlAsync(shortUrl);


        // Assert
        Assert.Equal(longUrl, result);
    }

    [Fact]
    public async Task CreateShortUrlAsyncTests()
    {
        // Arrange
        var url = GetUrl();
        var longUrl = url.First().LongUrl;
        var shortUrl = url.First().ShortUrl;

        _mockDbContext.Setup(x => x.Url).ReturnsDbSet(url);

        // Act
        var result = await _urlServices.CreateShortUrlAsync(longUrl);

        // Assert
        Assert.Equal(shortUrl, result);
    }
    #endregion

    #region Helpers
    private List<Url> GetUrl()
    {
        var longUrl = "http://create_short_url_async_tests.ru";
        var shortUrl = "qwerty";

        var url = new List<Url>
        {
            new Url { ShortUrl = shortUrl, LongUrl = longUrl }
        };

        return url;
    }
    #endregion
}