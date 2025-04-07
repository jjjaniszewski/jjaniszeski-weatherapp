using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.Options;
using JJaniszewski.WeatherApp.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Shouldly;
using Xunit;

namespace JJaniszewski.WeatherApp.Infrastructure.UnitTests.Services;

public class WeatherApiClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly Mock<ILogger<WeatherApiClient>> _loggerMock;
    private readonly Mock<IOptions<WeatherApiOptions>> _optionsMock;

    public WeatherApiClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _loggerMock = new Mock<ILogger<WeatherApiClient>>();
        _optionsMock = new Mock<IOptions<WeatherApiOptions>>();
    }

    private WeatherApiClient CreateClient()
    {
        _optionsMock.Setup(x => x.Value)
            .Returns(new WeatherApiOptions { ApiKey = "test-api-key" });

        return new WeatherApiClient(new HttpClient(_httpMessageHandlerMock.Object), _loggerMock.Object, _optionsMock.Object);
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_WhenApiCallSucceeds_ReturnsSuccessResult()
    {
        // Arrange
        var city = "London";
        var country = "UK";
        var expectedContent = "{\"weather\":[{\"main\":\"Clear\"}]}";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedContent)
            });

        var client = CreateClient();

        // Act
        var result = await client.GetCurrentWeatherAsync(city, country);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.City.ShouldBe(city);
        result.Country.ShouldBe(country);
        result.ResponseData.ShouldBe(expectedContent);
        result.ErrorMessage.ShouldBeNull();

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Successfully fetched weather data for {city},{country}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_WhenApiCallFails_ReturnsFailureResult()
    {
        // Arrange
        var city = "London";
        var country = "UK";
        var errorContent = "{\"message\":\"City not found\"}";
        var requestDate = DateTimeOffset.UtcNow;

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(errorContent)
            });

        var client = CreateClient();

        // Act
        var result = await client.GetCurrentWeatherAsync(city, country);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.City.ShouldBe(city);
        result.Country.ShouldBe(country);
        result.ResponseData.ShouldBe(errorContent);
        result.ErrorMessage.ShouldBe($"HTTP {HttpStatusCode.NotFound}, content: {errorContent}");

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Weather API returned non-success status for {city},{country}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetCurrentWeatherAsync_WhenExceptionOccurs_ReturnsFailureResult()
    {
        // Arrange
        var city = "London";
        var country = "UK";
        var exceptionMessage = "Network error";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException(exceptionMessage));

        var client = CreateClient();

        // Act
        var result = await client.GetCurrentWeatherAsync(city, country);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.City.ShouldBe(city);
        result.Country.ShouldBe(country);
        result.ResponseData.ShouldBeNull();
        result.ErrorMessage.ShouldBe(exceptionMessage);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Failed to fetch weather for {city},{country}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
