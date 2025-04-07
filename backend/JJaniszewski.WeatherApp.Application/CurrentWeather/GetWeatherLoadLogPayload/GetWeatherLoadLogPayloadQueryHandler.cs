using System;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.Services;
using MediatR;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.GetWeatherLoadLogPayload;

/// <summary>
/// Handler for the GetWeatherLoadLogPayloadQuery
/// </summary>
public class GetWeatherLoadLogPayloadQueryHandler : IRequestHandler<GetWeatherLoadLogPayloadQuery, string>
{
    private readonly IWeatherLoadLogsTableClient _weatherLoadLogsTableClient;
    private readonly IWeatherRecordPayloadsBlobClient _weatherRecordPayloadsBlobClient;

    /// <summary>
    /// Initializes a new instance of the GetWeatherLoadLogPayloadQueryHandler class
    /// </summary>
    /// <param name="weatherLoadLogsTableClient">The client for accessing weather load logs table storage</param>
    /// <param name="weatherRecordPayloadsBlobClient">The client for accessing weather record payloads blob storage</param>
    public GetWeatherLoadLogPayloadQueryHandler(
        IWeatherLoadLogsTableClient weatherLoadLogsTableClient,
        IWeatherRecordPayloadsBlobClient weatherRecordPayloadsBlobClient)
    {
        _weatherLoadLogsTableClient = weatherLoadLogsTableClient;
        _weatherRecordPayloadsBlobClient = weatherRecordPayloadsBlobClient;
    }

    /// <summary>
    /// Handles the query to get weather load log payload by its ID
    /// </summary>
    /// <param name="request">The query containing the ID</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The payload content as a string</returns>
    public async Task<string> Handle(GetWeatherLoadLogPayloadQuery request, CancellationToken cancellationToken)
    {
        // First, get the entity from table storage, as improvement we could create partitionKey from rowKey
        var entity = await _weatherLoadLogsTableClient.GetAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException($"Weather load log with ID {request.Id} not found");
        }

        if (string.IsNullOrEmpty(entity.PayloadBlobReference))
        {
            throw new InvalidOperationException($"Weather load log with ID {request.Id} has no payload reference");
        }

        return await _weatherRecordPayloadsBlobClient.GetAsync(entity.PayloadBlobReference, cancellationToken);
    }
}
