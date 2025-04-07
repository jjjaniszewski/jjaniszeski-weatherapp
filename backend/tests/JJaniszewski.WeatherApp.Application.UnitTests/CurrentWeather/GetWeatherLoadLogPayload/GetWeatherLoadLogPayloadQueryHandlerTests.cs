using System;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.CurrentWeather.GetWeatherLoadLogPayload;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Application.Services;
using Moq;
using Shouldly;
using Xunit;

namespace JJaniszewski.WeatherApp.Application.UnitTests.CurrentWeather.GetWeatherLoadLogPayload;

public class GetWeatherLoadLogPayloadQueryHandlerTests
{
    private readonly Mock<IWeatherLoadLogsTableClient> _weatherLoadLogsTableClientMock;
    private readonly Mock<IWeatherRecordPayloadsBlobClient> _weatherRecordPayloadsBlobClientMock;

    public GetWeatherLoadLogPayloadQueryHandlerTests()
    {
        _weatherLoadLogsTableClientMock = new Mock<IWeatherLoadLogsTableClient>();
        _weatherRecordPayloadsBlobClientMock = new Mock<IWeatherRecordPayloadsBlobClient>();
    }

    private GetWeatherLoadLogPayloadQueryHandler CreateHandler()
    {
        return new GetWeatherLoadLogPayloadQueryHandler(
            _weatherLoadLogsTableClientMock.Object,
            _weatherRecordPayloadsBlobClientMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEntityExistsAndHasPayloadReference_ReturnsPayload()
    {
        // Arrange
        var id = "test-id";
        var payloadReference = "blob-reference";
        var expectedPayload = "test payload content";

        var entity = new WeatherLoadLogEntity(
            "test-city",
            "test-country",
            "Success",
            DateTimeOffset.UtcNow,
            payloadReference);

        _weatherLoadLogsTableClientMock
            .Setup(x => x.GetAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _weatherRecordPayloadsBlobClientMock
            .Setup(x => x.GetAsync(payloadReference, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPayload);

        var query = new GetWeatherLoadLogPayloadQuery(id);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedPayload);
        _weatherLoadLogsTableClientMock.Verify(x => x.GetAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _weatherRecordPayloadsBlobClientMock.Verify(x => x.GetAsync(payloadReference, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEntityNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = "non-existent-id";

        _weatherLoadLogsTableClientMock
            .Setup(x => x.GetAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeatherLoadLogEntity?) null);

        var query = new GetWeatherLoadLogPayloadQuery(id);
        var handler = CreateHandler();

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() =>
            handler.Handle(query, CancellationToken.None));

        _weatherLoadLogsTableClientMock.Verify(x => x.GetAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _weatherRecordPayloadsBlobClientMock.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEntityExistsButNoPayloadReference_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = "test-id";

        var entity = new WeatherLoadLogEntity(
            "test-city",
            "test-country",
            "Success",
            DateTimeOffset.UtcNow);

        _weatherLoadLogsTableClientMock
            .Setup(x => x.GetAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var query = new GetWeatherLoadLogPayloadQuery(id);
        var handler = CreateHandler();

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() =>
            handler.Handle(query, CancellationToken.None));

        _weatherLoadLogsTableClientMock.Verify(x => x.GetAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _weatherRecordPayloadsBlobClientMock.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
