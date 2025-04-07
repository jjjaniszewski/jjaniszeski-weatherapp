using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.CurrentWeather.LoadWeatherRecord;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JJaniszewski.WeatherApp.AzureFunction.Functions;

public class TimerWeatherUpdaterFunction
{
    private readonly List<CityDto> _cities =
    [
        new ("London", "uk"),
        new ("Liverpool", "uk"),
        new ("Warsaw", "pl"),
        new ("Krakow", "pl"),
        new ("New York", "us"),
        new ("Washington", "us")
    ];

    private readonly ILogger<TimerWeatherUpdaterFunction> _logger;
    private readonly IMediator _mediator;

    public TimerWeatherUpdaterFunction(ILogger<TimerWeatherUpdaterFunction> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Function(nameof(TimerWeatherUpdaterFunction))]
    public async Task Run([TimerTrigger("%WEATHER_UPDATER_CRON_SCHEDULE%")] TimerInfo myTimer)
    {
        _logger.LogInformation("Starting weather updater at {Now} with {CitiesCount} cities", DateTime.Now, _cities.Count);

        var i = 0;
        foreach (var cityDto in _cities)
        {
            await _mediator.Send(new LoadWeatherRecordCommand(cityDto.City, cityDto.Country));
            i++;
            _logger.LogInformation("Updated weather for {City} {Country} at {Now} [{CurrentIndex}/{CitiesCount} cities]", cityDto.City, cityDto.Country, DateTime.Now, i, _cities.Count);
        }

        _logger.LogInformation("Finished weather updater at {Now} with {CitiesCount} cities", DateTime.Now, _cities.Count);

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Weather updater next timer schedule at: {ScheduleStatusNext}", myTimer.ScheduleStatus.Next);
        }
    }

    private record CityDto(string City, string Country);
}
