using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Domain.Repositories;
using MediatR;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.Queries.GetAllWeatherRecords;

public class GetAllWeatherRecordsQueryHandler : IRequestHandler<GetAllWeatherRecordsQuery, IEnumerable<WeatherRecordDto>>
{
    private readonly IWeatherRecordRepository _weatherRecordRepository;

    public GetAllWeatherRecordsQueryHandler(IWeatherRecordRepository weatherRecordRepository)
    {
        _weatherRecordRepository = weatherRecordRepository;
    }

    public async Task<IEnumerable<WeatherRecordDto>> Handle(GetAllWeatherRecordsQuery request, CancellationToken cancellationToken)
    {
        var records = await _weatherRecordRepository.GetAllAsync(cancellationToken);

        return records.Select(record => new WeatherRecordDto
        {
            Id = record.Id,
            City = record.City,
            Country = record.Country,
            DateUtc = record.DateUtc,
            RequestDateUtc = record.RequestDateUtc,
            Temperature = record.Temperature,
            MinTemperature = record.MinTemperature,
            MaxTemperature = record.MaxTemperature
        });
    }
}
