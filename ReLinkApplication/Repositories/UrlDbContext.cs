using Microsoft.EntityFrameworkCore;
using ReLinkApplication.Models;

namespace ReLinkApplication.Repositories;

public class UrlDbContext : DbContext
{
    public DbSet<Url> Url { get; set; }

    public UrlDbContext()
    {
    }

    public UrlDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Url>().ToTable("ReLink").HasKey(x => x.Id);
        modelBuilder.Entity<Url>().Property(x => x.Id).ValueGeneratedOnAdd();

        modelBuilder.Entity<Url>().Property(x => x.LongUrl).HasColumnName("LongUrl").IsRequired().HasMaxLength(2048);
        modelBuilder.Entity<Url>().Property(x => x.ShortUrl).HasColumnName("ShortUrl").IsRequired().HasMaxLength(100);

        base.OnModelCreating(modelBuilder);
    }
}
