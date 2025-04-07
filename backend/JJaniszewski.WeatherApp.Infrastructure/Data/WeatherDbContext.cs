using JJaniszewski.WeatherApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JJaniszewski.WeatherApp.Infrastructure.Data;

/// <summary>
/// Database context class for managing weather-related data using Entity Framework Core
/// </summary>
public class WeatherDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the WeatherDbContext class
    /// </summary>
    /// <param name="options">The options to be used by the DbContext</param>
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the DbSet of WeatherRecord entities
    /// </summary>
    public DbSet<WeatherRecord> WeatherRecords { get; set; }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in Microsoft.EntityFrameworkCore.DbSet`1 properties on your derived context
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.Country).IsRequired();
            entity.Property(e => e.RequestDateUtc).IsRequired();
            entity.Property(e => e.DateUtc).IsRequired();
        });
    }
}
