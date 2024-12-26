using Microsoft.EntityFrameworkCore;
using ReLinkApplication.Repositories;
using ReLinkApplication.Services;

namespace ReLinkApplication.Tests;

public class UrlServicesTests
{

    [Fact]
    public async Task CreateShortLongUrl()
    {
        //Arrange
        using var dbContext = CreateInMemoryDbContext();
        var urlService = new UrlServices(dbContext);

        var longUrl = "http://test.url.ru";

        //Act
        var shortUrl = await urlService.CreateShortUrlAsync(longUrl);    
        var retrievedlongUrl = await urlService.GetLongUrlByShortUrlAsync(shortUrl);

        //Assert
        Assert.NotNull(shortUrl);
        Assert.Equal(longUrl, retrievedlongUrl);
    }

    private UrlDbContext CreateInMemoryDbContext()
    {
        var dbContext = new DbContextOptionsBuilder<UrlDbContext>()
            .UseInMemoryDatabase(databaseName: "RelinkTestDb")
            .Options;

        return new UrlDbContext(dbContext);
    }
}

