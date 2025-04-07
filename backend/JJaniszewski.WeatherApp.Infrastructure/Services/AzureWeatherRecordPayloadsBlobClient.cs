using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using JJaniszewski.WeatherApp.Application.Services;

namespace JJaniszewski.WeatherApp.Infrastructure.Services;

/// <inheritdoc />
public class AzureWeatherRecordPayloadsBlobClient : IWeatherRecordPayloadsBlobClient
{
    public const string CONTAINER_NAME = "weatherloadpayloads";
    private readonly BlobContainerClient _containerClient;

    public AzureWeatherRecordPayloadsBlobClient(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    /// <inheritdoc />
    public async Task CreateAsync(string blobName, string content, CancellationToken cancellationToken)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await blobClient.UploadAsync(stream, true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> GetAsync(string blobName, CancellationToken cancellationToken)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        var response = await blobClient.DownloadContentAsync(cancellationToken);
        return response.Value.Content.ToString();
    }
}
