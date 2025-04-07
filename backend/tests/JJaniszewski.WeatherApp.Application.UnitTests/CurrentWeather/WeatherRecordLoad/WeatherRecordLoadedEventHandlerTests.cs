using System;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.CurrentWeather.WeatherRecordLoad;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Application.Services;
using Moq;
using Xunit;

namespace JJaniszewski.WeatherApp.Application.UnitTests.CurrentWeather.WeatherRecordLoad;

public class WeatherRecordLoadedEventHandlerTests
{
    private readonly Mock<IWeatherRecordPayloadsBlobClient> _blobClientMock;
    private readonly Mock<IWeatherLoadLogsTableClient> _tableClientMock;

    public WeatherRecordLoadedEventHandlerTests()
    {
        _tableClientMock = new Mock<IWeatherLoadLogsTableClient>();
        _blobClientMock = new Mock<IWeatherRecordPayloadsBlobClient>();
    }

    [Fact]
    public async Task Handle_ShouldWriteToBlobStorage()
    {
        // Arrange
        var handler = new WeatherRecordLoadedEventHandler(_blobClientMock.Object, _tableClientMock.Object);
        var weatherLoadResult = WeatherLoadResult.Success("London", "UK", DateTimeOffset.UtcNow, "{\"test\":\"data\"}");
        var @event = new WeatherRecordLoadedEvent("London", "UK", weatherLoadResult);
        var expectedBlobPath = weatherLoadResult.GetBlobPath();

        // Act
        await handler.Handle(@event, CancellationToken.None);

        // Assert
        _blobClientMock.Verify(x => x.CreateAsync(
                expectedBlobPath,
                weatherLoadResult.ResponseData,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateLogEntity()
    {
        // Arrange
        var date = DateTimeOffset.UtcNow;
        var handler = new WeatherRecordLoadedEventHandler(_blobClientMock.Object, _tableClientMock.Object);
        var weatherLoadResult = WeatherLoadResult.Success("London", "UK", date, "{\"test\":\"data\"}");
        var @event = new WeatherRecordLoadedEvent("London", "UK", weatherLoadResult);
        var expectedBlobPath = weatherLoadResult.GetBlobPath();

        // Act
        await handler.Handle(@event, CancellationToken.None);

        // Assert
        _tableClientMock.Verify<Task>(x => x.CreateAsync(
                It.Is<WeatherLoadLogEntity>(entity =>
                    entity.PartitionKey.StartsWith("WeatherLoadLogs_") &&
                    entity.Status == "Success" &&
                    entity.PayloadBlobReference == expectedBlobPath &&
                    entity.Date == date &&
                    string.IsNullOrEmpty(entity.Reason)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateLogEntityWithCorrectProperties()
    {
        // Arrange
        var handler = new WeatherRecordLoadedEventHandler(_blobClientMock.Object, _tableClientMock.Object);
        var date = DateTimeOffset.UtcNow;
        var weatherLoadResult = WeatherLoadResult.Success("London", "UK", date, "{\"test\":\"data\"}");
        var @event = new WeatherRecordLoadedEvent("London", "UK", weatherLoadResult);
        var expectedBlobPath = weatherLoadResult.GetBlobPath();

        // Act
        await handler.Handle(@event, CancellationToken.None);

        // Assert
        _tableClientMock.Verify<Task>(x => x.CreateAsync(
                It.Is<WeatherLoadLogEntity>(entity =>
                    entity.PartitionKey.StartsWith("WeatherLoadLogs_") &&
                    entity.Status == "Success" &&
                    entity.PayloadBlobReference == expectedBlobPath &&
                    entity.Date == date &&
                    string.IsNullOrEmpty(entity.Reason) &&
                    entity.RowKey.StartsWith($"{entity.City}_{entity.Country}_{date:yyyyMMddHHmmssfff}")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
