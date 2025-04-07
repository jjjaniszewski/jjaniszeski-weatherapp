using System.Collections.Generic;
using JJaniszewski.WeatherApp.Application.Models;
using MediatR;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.Queries.GetAllWeatherRecords;

public record GetAllWeatherRecordsQuery : IRequest<IEnumerable<WeatherRecordDto>>;
