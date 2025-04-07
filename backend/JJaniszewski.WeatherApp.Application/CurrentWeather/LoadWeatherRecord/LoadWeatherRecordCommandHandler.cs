using System;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.CurrentWeather.WeatherRecordLoad;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Application.Services;
using JJaniszewski.WeatherApp.Domain.Entities;
using JJaniszewski.WeatherApp.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.LoadWeatherRecord;

/// <summary>
/// Handles the command to load current weather record for a specific city and country.
/// </summary>
public class LoadWeatherRecordCommandHandler : IRequestHandler<LoadWeatherRecordCommand, Unit>
{
    private readonly ILogger<LoadWeatherRecordCommandHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IWeatherApiClient _weatherApiClient;
    private readonly IWeatherRecordRepository _weatherRecordRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadWeatherRecordCommandHandler" /> class.
    /// </summary>
    /// <param name="mediator">The mediator for publishing events.</param>
    /// <param name="weatherApiClient">The weather API client for fetching weather data.</param>
    /// <param name="weatherRecordRepository">The repository for storing weather records.</param>
    /// <param name="logger">The logger for recording events and errors.</param>
    public LoadWeatherRecordCommandHandler(
        IMediator mediator,
        IWeatherApiClient weatherApiClient,
        IWeatherRecordRepository weatherRecordRepository,
        ILogger<LoadWeatherRecordCommandHandler> logger)
    {
        _mediator = mediator;
        _weatherApiClient = weatherApiClient;
        _weatherRecordRepository = weatherRecordRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the command to load weather data for a specific city and country.
    /// </summary>
    /// <param name="request">The command containing city and country information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Unit> Handle(LoadWeatherRecordCommand request, CancellationToken cancellationToken)
    {
        var weatherResult = await _weatherApiClient.GetCurrentWeatherAsync(request.City, request.Country);

        INotification resultEvent = weatherResult.IsSuccess
            ? new WeatherRecordLoadedEvent(request.City, request.Country, weatherResult)
            : new WeatherRecordLoadFailureEvent(request.City, request.Country, weatherResult);

        if (weatherResult.IsSuccess && weatherResult.ParsedResponse != null)
        {
            try
            {
                // Create and save database record using the parsed response
                var record = WeatherRecord.Create(
                    request.City,
                    request.Country,
                    weatherResult.ParsedResponse.DateTime,
                    weatherResult.RequestDateUtc,
                    weatherResult.ParsedResponse.Main.Temp,
                    weatherResult.ParsedResponse.Main.Temp_Min,
                    weatherResult.ParsedResponse.Main.Temp_Max);

                await _weatherRecordRepository.AddAsync(record, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not read weather for {City},{Country} with {ExceptionMessage}", request.City, request.Country, ex.Message);
                resultEvent = new WeatherRecordLoadFailureEvent(request.City, request.Country, WeatherLoadResult.Failure(weatherResult.City, weatherResult.Country, weatherResult.RequestDateUtc, weatherResult.ResponseData, ex.Message));
            }
        }

        await _mediator.Publish(resultEvent, cancellationToken);

        return Unit.Value;
    }
}
