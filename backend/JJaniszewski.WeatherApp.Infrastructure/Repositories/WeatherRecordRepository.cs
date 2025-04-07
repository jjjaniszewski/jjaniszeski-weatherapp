using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Domain.Entities;
using JJaniszewski.WeatherApp.Domain.Repositories;
using JJaniszewski.WeatherApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JJaniszewski.WeatherApp.Infrastructure.Repositories;

/// <inheritdoc />
public class WeatherRecordRepository : IWeatherRecordRepository
{
    private readonly WeatherDbContext _context;

    public WeatherRecordRepository(WeatherDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(WeatherRecord record, CancellationToken cancellationToken)
    {
        await _context.WeatherRecords.AddAsync(record, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WeatherRecord>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.WeatherRecords.ToListAsync(cancellationToken);
    }
}
