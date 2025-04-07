using System;

namespace JJaniszewski.WeatherApp.Application.Models;

public class WeatherRecordDto
{
    public Guid Id { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public DateTimeOffset DateUtc { get; set; }
    public DateTimeOffset RequestDateUtc { get; set; }
    public double MinTemperature { get; set; }
    public double MaxTemperature { get; set; }
    public double Temperature { get; set; }
}
