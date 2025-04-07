using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.CurrentWeather.GetWeatherLoadLogs;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Application.Services;
using Moq;
using Shouldly;
using Xunit;

namespace JJaniszewski.WeatherApp.Application.UnitTests.CurrentWeather.GetWeatherLoadLogs;

public class GetWeatherLoadLogsQueryHandlerTests
{
    private readonly Mock<IWeatherLoadLogsTableClient> _weatherLoadLogsTableClientMock;

    public GetWeatherLoadLogsQueryHandlerTests()
    {
        _weatherLoadLogsTableClientMock = new Mock<IWeatherLoadLogsTableClient>();
    }

    private GetWeatherLoadLogsQueryHandler CreateHandler()
    {
        return new GetWeatherLoadLogsQueryHandler(_weatherLoadLogsTableClientMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEntitiesExist_ReturnsMappedResponses()
    {
        // Arrange
        var fromDate = DateTimeOffset.UtcNow.AddDays(-1);
        var toDate = DateTimeOffset.UtcNow;

        var entities = new List<WeatherLoadLogEntity>
        {
            new (
                "test-city-1",
                "test-country-1",
                "Success",
                fromDate.AddHours(1),
                "blob-ref-1"),
            new (
                "test-city-2",
                "test-country-2",
                "Failed",
                fromDate.AddHours(2),
                "blob-ref-2",
                "Error occurred")
        };

        _weatherLoadLogsTableClientMock
            .Setup(x => x.QueryByDateRangeAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        var query = new GetWeatherLoadLogsQuery(fromDate, toDate);
        var handler = CreateHandler();

        // Act
        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<List<WeatherLoadLogResponse>>();
        result.Count().ShouldBe(2);

        var firstResponse = result[0];
        firstResponse.Id.ShouldBe(entities[0].RowKey);
        firstResponse.City.ShouldBe(entities[0].City);
        firstResponse.Country.ShouldBe(entities[0].Country);
        firstResponse.Status.ShouldBe(entities[0].Status);
        firstResponse.Date.ShouldBe(entities[0].Date);
        firstResponse.Reason.ShouldBe(entities[0].Reason);

        var secondResponse = result[1];
        secondResponse.Id.ShouldBe(entities[1].RowKey);
        secondResponse.City.ShouldBe(entities[1].City);
        secondResponse.Country.ShouldBe(entities[1].Country);
        secondResponse.Status.ShouldBe(entities[1].Status);
        secondResponse.Date.ShouldBe(entities[1].Date);
        secondResponse.Reason.ShouldBe(entities[1].Reason);

        _weatherLoadLogsTableClientMock.Verify(
            x => x.QueryByDateRangeAsync(fromDate, toDate, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNoEntitiesExist_ReturnsEmptyCollection()
    {
        // Arrange
        var fromDate = DateTimeOffset.UtcNow.AddDays(-1);
        var toDate = DateTimeOffset.UtcNow;

        _weatherLoadLogsTableClientMock
            .Setup(x => x.QueryByDateRangeAsync(fromDate, toDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<WeatherLoadLogEntity>());

        var query = new GetWeatherLoadLogsQuery(fromDate, toDate);
        var handler = CreateHandler();

        // Act
        var result = (await handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<List<WeatherLoadLogResponse>>();
        result.ShouldBeEmpty();

        _weatherLoadLogsTableClientMock.Verify(
            x => x.QueryByDateRangeAsync(fromDate, toDate, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
