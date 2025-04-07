using System;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.CurrentWeather.WeatherRecordLoad;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Application.Services;
using Moq;
using Xunit;

namespace JJaniszewski.WeatherApp.Application.UnitTests.CurrentWeather.WeatherRecordLoad;

public class WeatherRecordLoadFailureEventHandlerTests
{
    private readonly Mock<IWeatherLoadLogsTableClient> _tableClientMock;

    public WeatherRecordLoadFailureEventHandlerTests()
    {
        _tableClientMock = new Mock<IWeatherLoadLogsTableClient>();
    }

    [Fact]
    public async Task Handle_ShouldCreateLogEntity()
    {
        // Arrange
        var handler = new WeatherRecordLoadFailureEventHandler(_tableClientMock.Object);
        var date = DateTimeOffset.UtcNow;
        var weatherLoadResult = WeatherLoadResult.Failure("London", "UK", date, "error response", "Test error");
        var weatherEvent = new WeatherRecordLoadFailureEvent("London", "UK", weatherLoadResult);

        // Act
        await handler.Handle(weatherEvent, CancellationToken.None);

        // Assert
        _tableClientMock.Verify<Task>(x => x.CreateAsync(
                It.Is<WeatherLoadLogEntity>(entity =>
                    entity.PartitionKey.StartsWith("WeatherLoadLogs_") &&
                    entity.Status == "Failure"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateLogEntityWithErrorReason()
    {
        // Arrange
        var date = DateTimeOffset.UtcNow;
        var handler = new WeatherRecordLoadFailureEventHandler(_tableClientMock.Object);
        var weatherLoadResult = WeatherLoadResult.Failure("London", "UK", date, "error response", "Test error");
        var @event = new WeatherRecordLoadFailureEvent("London", "UK", weatherLoadResult);

        // Act
        await handler.Handle(@event, CancellationToken.None);

        // Assert
        _tableClientMock.Verify(x => x.CreateAsync(
                It.Is<WeatherLoadLogEntity>(entity =>
                    entity.PartitionKey.StartsWith("WeatherLoadLogs_") &&
                    entity.Status == "Failure" &&
                    entity.Reason == "Test error" &&
                    entity.RowKey.StartsWith($"{entity.City}_{entity.Country}_{date:yyyyMMddHHmmssfff}")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateLogEntityWithoutBlobReference()
    {
        // Arrange
        var date = DateTimeOffset.UtcNow;
        var handler = new WeatherRecordLoadFailureEventHandler(_tableClientMock.Object);
        var weatherLoadResult = WeatherLoadResult.Failure("London", "UK", date, "error response", "Test error");
        var @event = new WeatherRecordLoadFailureEvent("London", "UK", weatherLoadResult);

        // Act
        await handler.Handle(@event, CancellationToken.None);

        // Assert
        _tableClientMock.Verify(x => x.CreateAsync(
                It.Is<WeatherLoadLogEntity>(entity =>
                    entity.PartitionKey.StartsWith("WeatherLoadLogs_") &&
                    entity.Status == "Failure" &&
                    string.IsNullOrEmpty(entity.PayloadBlobReference)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
