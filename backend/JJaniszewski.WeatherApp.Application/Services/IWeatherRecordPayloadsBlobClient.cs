using System.Threading;
using System.Threading.Tasks;

namespace JJaniszewski.WeatherApp.Application.Services;

/// <summary>
/// Represents a client for handling weather record payloads in Azure Blob Storage.
/// </summary>
public interface IWeatherRecordPayloadsBlobClient
{
    /// <summary>
    /// Creates a new weather record payload in Azure Blob Storage.
    /// </summary>
    /// <param name="blobName">The name of the blob to create.</param>
    /// <param name="content">The content to write to the blob.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    Task CreateAsync(string blobName, string content, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the content of a weather record payload from Azure Blob Storage.
    /// </summary>
    /// <param name="blobName">The name of the blob to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The content of the blob as a string.</returns>
    Task<string> GetAsync(string blobName, CancellationToken cancellationToken);
}
