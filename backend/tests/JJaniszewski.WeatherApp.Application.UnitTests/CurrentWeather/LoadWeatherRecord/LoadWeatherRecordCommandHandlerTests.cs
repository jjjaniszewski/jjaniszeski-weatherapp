using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.CurrentWeather.LoadWeatherRecord;
using JJaniszewski.WeatherApp.Application.CurrentWeather.WeatherRecordLoad;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Application.Services;
using JJaniszewski.WeatherApp.Domain.Entities;
using JJaniszewski.WeatherApp.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace JJaniszewski.WeatherApp.Application.UnitTests.CurrentWeather.LoadWeatherRecord;

public class LoadWeatherRecordCommandHandlerTests
{
    private const string TestCity = "London";
    private const string TestCountry = "uk";
    private static readonly DateTimeOffset TestDate = DateTimeOffset.UtcNow;

    private LoadWeatherRecordCommandHandler CreateHandler(
        Mock<IMediator>? mediatorMock = null,
        Mock<IWeatherApiClient>? weatherApiClientMock = null,
        Mock<IWeatherRecordRepository>? weatherRecordRepositoryMock = null,
        Mock<ILogger<LoadWeatherRecordCommandHandler>>? loggerMock = null)
    {
        mediatorMock ??= new Mock<IMediator>();
        weatherApiClientMock ??= new Mock<IWeatherApiClient>();
        weatherRecordRepositoryMock ??= new Mock<IWeatherRecordRepository>();
        loggerMock ??= new Mock<ILogger<LoadWeatherRecordCommandHandler>>();

        return new LoadWeatherRecordCommandHandler(
            mediatorMock.Object,
            weatherApiClientMock.Object,
            weatherRecordRepositoryMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task Handle_SuccessfulWeatherLoad_CreatesRecordAndPublishesSuccessEvent()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var weatherApiClientMock = new Mock<IWeatherApiClient>();
        var weatherRecordRepositoryMock = new Mock<IWeatherRecordRepository>();
        var loggerMock = new Mock<ILogger<LoadWeatherRecordCommandHandler>>();

        var command = new LoadWeatherRecordCommand(TestCity, TestCountry);
        var weatherResponse = new WeatherApiResponse
        {
            Name = TestCity,
            Sys = new Sys { Country = TestCountry },
            Timestamp = TestDate.ToUnixTimeSeconds()
        };
        var responseData = JsonSerializer.Serialize(weatherResponse);
        var weatherResult = WeatherLoadResult.Success(TestCity, TestCountry, TestDate, responseData);

        weatherApiClientMock
            .Setup(x => x.GetCurrentWeatherAsync(TestCity, TestCountry))
            .ReturnsAsync(weatherResult);

        weatherRecordRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<WeatherRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = CreateHandler(mediatorMock, weatherApiClientMock, weatherRecordRepositoryMock, loggerMock);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        weatherApiClientMock.Verify(x => x.GetCurrentWeatherAsync(TestCity, TestCountry), Times.Once);
        weatherRecordRepositoryMock.Verify(x => x.AddAsync(
            It.Is<WeatherRecord>(r =>
                r.City == TestCity &&
                r.Country == TestCountry &&
                r.DateUtc == DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Timestamp) &&
                r.RequestDateUtc == TestDate),
            It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(x => x.Publish(
            It.Is<INotification>(e =>
                e as WeatherRecordLoadedEvent != null &&
                (e as WeatherRecordLoadedEvent)!.City == TestCity &&
                (e as WeatherRecordLoadedEvent)!.Country == TestCountry &&
                (e as WeatherRecordLoadedEvent)!.WeatherLoadResult == weatherResult),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FailedWeatherLoad_PublishesFailureEvent()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var weatherApiClientMock = new Mock<IWeatherApiClient>();
        var weatherRecordRepositoryMock = new Mock<IWeatherRecordRepository>();
        var loggerMock = new Mock<ILogger<LoadWeatherRecordCommandHandler>>();

        var command = new LoadWeatherRecordCommand(TestCity, TestCountry);
        const string errorMessage = "API Error";
        var weatherResult = WeatherLoadResult.Failure(TestCity, TestCountry, TestDate, null, errorMessage);

        weatherApiClientMock
            .Setup(x => x.GetCurrentWeatherAsync(TestCity, TestCountry))
            .ReturnsAsync(weatherResult);

        var handler = CreateHandler(mediatorMock, weatherApiClientMock, weatherRecordRepositoryMock, loggerMock);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        weatherApiClientMock.Verify(x => x.GetCurrentWeatherAsync(TestCity, TestCountry), Times.Once);
        weatherRecordRepositoryMock.Verify(x => x.AddAsync(It.IsAny<WeatherRecord>(), It.IsAny<CancellationToken>()), Times.Never);
        mediatorMock.Verify(x => x.Publish(
            It.Is<INotification>(e =>
                e as WeatherRecordLoadFailureEvent != null &&
                (e as WeatherRecordLoadFailureEvent)!.City == TestCity &&
                (e as WeatherRecordLoadFailureEvent)!.Country == TestCountry &&
                (e as WeatherRecordLoadFailureEvent)!.WeatherLoadResult == weatherResult),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DatabaseError_PublishesFailureEvent()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var weatherApiClientMock = new Mock<IWeatherApiClient>();
        var weatherRecordRepositoryMock = new Mock<IWeatherRecordRepository>();
        var loggerMock = new Mock<ILogger<LoadWeatherRecordCommandHandler>>();

        var command = new LoadWeatherRecordCommand(TestCity, TestCountry);
        var weatherResponse = new WeatherApiResponse
        {
            Name = TestCity,
            Sys = new Sys { Country = TestCountry },
            Timestamp = TestDate.ToUnixTimeSeconds()
        };
        var responseData = JsonSerializer.Serialize(weatherResponse);
        var weatherResult = WeatherLoadResult.Success(TestCity, TestCountry, TestDate, responseData);
        const string dbError = "Database Error";

        weatherApiClientMock
            .Setup(x => x.GetCurrentWeatherAsync(TestCity, TestCountry))
            .ReturnsAsync(weatherResult);

        weatherRecordRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<WeatherRecord>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(dbError));

        mediatorMock.Setup(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()));

        var handler = CreateHandler(mediatorMock, weatherApiClientMock, weatherRecordRepositoryMock, loggerMock);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        weatherApiClientMock.Verify(x => x.GetCurrentWeatherAsync(TestCity, TestCountry), Times.Once);
        weatherRecordRepositoryMock.Verify(x => x.AddAsync(It.IsAny<WeatherRecord>(), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(x => x.Publish(
            It.Is<INotification>(e =>
                e as WeatherRecordLoadFailureEvent != null &&
                (e as WeatherRecordLoadFailureEvent)!.City == TestCity &&
                (e as WeatherRecordLoadFailureEvent)!.Country == TestCountry &&
                (e as WeatherRecordLoadFailureEvent)!.WeatherLoadResult.IsSuccess == false &&
                (e as WeatherRecordLoadFailureEvent)!.WeatherLoadResult.ErrorMessage == dbError &&
                (e as WeatherRecordLoadFailureEvent)!.WeatherLoadResult.ResponseData == responseData),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
