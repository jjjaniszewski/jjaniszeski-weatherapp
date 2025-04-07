using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Domain.Entities;

namespace JJaniszewski.WeatherApp.Domain.Repositories;

/// <summary>
/// Repository interface for managing weather records
/// </summary>
public interface IWeatherRecordRepository
{
    /// <summary>
    /// Asynchronously adds a new weather record to the repository
    /// </summary>
    /// <param name="record">The weather record to add</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AddAsync(WeatherRecord record, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves all weather records from the repository
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation</param>
    /// <returns>A collection of weather records</returns>
    Task<IEnumerable<WeatherRecord>> GetAllAsync(CancellationToken cancellationToken);
}
