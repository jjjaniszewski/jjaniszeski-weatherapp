using System;
using System.Text.Json;
using JJaniszewski.WeatherApp.Application.Models;
using Shouldly;
using Xunit;

namespace JJaniszewski.WeatherApp.Application.UnitTests.Services;

public class WeatherLoadResultTests
{
    private const string TestCity = "London";
    private const string TestCountry = "UK";
    private static readonly DateTimeOffset TestDate = DateTimeOffset.UtcNow;

    [Fact]
    public void Success_WithValidResponse_ReturnsSuccessResult()
    {
        // Arrange
        var responseData = JsonSerializer.Serialize(new WeatherApiResponse
        {
            Name = TestCity,
            Sys = new Sys { Country = TestCountry },
            Timestamp = TestDate.ToUnixTimeSeconds()
        });

        // Act
        var result = WeatherLoadResult.Success(TestCity, TestCountry, TestDate, responseData);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.City.ShouldBe(TestCity);
        result.Country.ShouldBe(TestCountry);
        result.RequestDateUtc.ShouldBe(TestDate);
        result.ResponseData.ShouldBe(responseData);
        result.ErrorMessage.ShouldBeNull();
        result.ParsedResponse.ShouldNotBeNull();
        result.ParsedResponse.Name.ShouldBe(TestCity);
        result.ParsedResponse.Sys.Country.ShouldBe(TestCountry);
    }

    [Fact]
    public void Success_WithInvalidJson_ThrowsJsonException()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act & Assert
        Should.Throw<JsonException>(() =>
            WeatherLoadResult.Success(TestCity, TestCountry, TestDate, invalidJson));
    }

    [Fact]
    public void Failure_ReturnsFailureResult()
    {
        // Arrange
        const string errorMessage = "Test error";
        const string responseData = "{}";

        // Act
        var result = WeatherLoadResult.Failure(TestCity, TestCountry, TestDate, responseData, errorMessage);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.City.ShouldBe(TestCity);
        result.Country.ShouldBe(TestCountry);
        result.RequestDateUtc.ShouldBe(TestDate);
        result.ResponseData.ShouldBe(responseData);
        result.ErrorMessage.ShouldBe(errorMessage);
        result.ParsedResponse.ShouldBeNull();
    }

    [Fact]
    public void GetBlobPath_ReturnsCorrectPath()
    {
        // Arrange
        var result = WeatherLoadResult.Success(TestCity, TestCountry, TestDate, "{}");
        var expectedPath = $"payloads/{TestCity}-{TestCountry}-{TestDate:yyyy-MM-dd_HH-mm-ss-fff}.json";

        // Act
        var path = result.GetBlobPath();

        // Assert
        path.ShouldBe(expectedPath);
    }

    [Fact]
    public void GetBlobPath_StaticMethod_ReturnsCorrectPath()
    {
        // Arrange
        var expectedPath = $"payloads/{TestCity}-{TestCountry}-{TestDate:yyyy-MM-dd_HH-mm-ss-fff}.json";

        // Act
        var path = WeatherLoadResult.GetBlobPath(TestCity, TestCountry, TestDate);

        // Assert
        path.ShouldBe(expectedPath);
    }
}
