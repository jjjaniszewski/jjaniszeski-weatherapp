using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Infrastructure.Data;
using JJaniszewski.WeatherApp.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace JJaniszewski.WeatherApp.Infrastructure.UnitTests.Services;

public class AzureWeatherLoadLogsTableClientTests
{
    private readonly Mock<ILogger<AzureWeatherLoadLogsTableClient>> _loggerMock;
    private readonly Mock<TableClient> _tableClientMock;

    public AzureWeatherLoadLogsTableClientTests()
    {
        _tableClientMock = new Mock<TableClient>();
        _loggerMock = new Mock<ILogger<AzureWeatherLoadLogsTableClient>>();
    }

    private AzureWeatherLoadLogsTableClient CreateClient()
    {
        return new AzureWeatherLoadLogsTableClient(_tableClientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_CreatesEntityInTable()
    {
        // Arrange
        var entity = new WeatherLoadLogEntity(
            "test-city",
            "test-country",
            "Success",
            DateTimeOffset.UtcNow,
            "blob-ref");

        _tableClientMock
            .Setup(x => x.AddEntityAsync(It.IsAny<TableEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response>());

        var client = CreateClient();

        // Act
        await client.CreateAsync(entity, CancellationToken.None);

        // Assert
        _tableClientMock.Verify(
            x => x.AddEntityAsync(
                It.Is<WeatherLoadLogTableEntity>(e =>
                    e.City == entity.City &&
                    e.Country == entity.Country &&
                    e.Status == entity.Status &&
                    e.Date == entity.Date &&
                    e.PayloadBlobReference == entity.PayloadBlobReference),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenEntityExists_ReturnsEntity()
    {
        // Arrange
        var rowKey = "test-row-key";
        var tableEntity = new TableEntity("test-partition", rowKey)
        {
            { "City", "test-city" },
            { "Country", "test-country" },
            { "Status", "Success" },
            { "Date", DateTimeOffset.UtcNow },
            { "PayloadBlobReference", "blob-ref" }
        };

        var asyncPageable = new Mock<AsyncPageable<TableEntity>>();
        asyncPageable.Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<TableEntity>(new[] { tableEntity }));

        _tableClientMock
            .Setup(x => x.QueryAsync<TableEntity>(
                It.Is<string>(f => f.Contains($"RowKey eq '{rowKey}'")),
                It.IsAny<int?>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
            .Returns(asyncPageable.Object);

        var client = CreateClient();

        // Act
        var result = await client.GetAsync(rowKey, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.City.ShouldBe(tableEntity.GetString("City"));
        result.Country.ShouldBe(tableEntity.GetString("Country"));
        result.Status.ShouldBe(tableEntity.GetString("Status"));
        result.Date.ShouldBe(tableEntity.GetDateTimeOffset("Date")!.Value);
        result.PayloadBlobReference.ShouldBe(tableEntity.GetString("PayloadBlobReference"));

        _tableClientMock.Verify(
            x => x.QueryAsync<TableEntity>(
                It.Is<string>(f => f.Contains($"RowKey eq '{rowKey}'")),
                It.IsAny<int?>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        // Arrange
        var rowKey = "non-existent-row-key";
        var asyncPageable = new Mock<AsyncPageable<TableEntity>>();
        var emptyList = Array.Empty<TableEntity>();
        asyncPageable.Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<TableEntity>(emptyList));

        _tableClientMock
            .Setup(x => x.QueryAsync<TableEntity>(
                It.Is<string>(f => f.Contains($"RowKey eq '{rowKey}'")),
                It.IsAny<int?>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
            .Returns(asyncPageable.Object);

        var client = CreateClient();

        // Act
        var result = await client.GetAsync(rowKey, CancellationToken.None);

        // Assert
        result.ShouldBeNull();
        _tableClientMock.Verify(
            x => x.QueryAsync<TableEntity>(
                It.Is<string>(f => f.Contains($"RowKey eq '{rowKey}'")),
                It.IsAny<int?>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task QueryByDateRangeAsync_WhenEntitiesExist_ReturnsEntities()
    {
        // Arrange
        var fromDate = DateTimeOffset.UtcNow.AddDays(-1);
        var toDate = DateTimeOffset.UtcNow;
        var partitionKey = $"WeatherLoadLogs_{fromDate:yyyyMMdd}";
        var tableEntity = new TableEntity(partitionKey, "test-row-key")
        {
            { "City", "test-city" },
            { "Country", "test-country" },
            { "Status", "Success" },
            { "Date", fromDate.AddHours(1) },
            { "PayloadBlobReference", "blob-ref" }
        };

        var entities = new[] { tableEntity };
        var asyncPageable = new Mock<AsyncPageable<TableEntity>>();
        asyncPageable.Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(() => new TestAsyncEnumerator<TableEntity>(entities));

        var emptyAsyncPageable = new Mock<AsyncPageable<TableEntity>>();
        emptyAsyncPageable.Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(() => new TestAsyncEnumerator<TableEntity>([]));

        // Only for this specific date (first) return one object, for rest empty enumerable
        _tableClientMock
            .Setup(x => x.QueryAsync<TableEntity>(
                It.Is<string>(f => f != $"PartitionKey eq '{partitionKey}' and Date ge datetime'{fromDate:yyyy-MM-ddTHH:mm:ss.fffZ}' and Date le datetime'{toDate:yyyy-MM-ddTHH:mm:ss.fffZ}'"),
                It.IsAny<int?>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
            .Returns(emptyAsyncPageable.Object);

        _tableClientMock
            .Setup(x => x.QueryAsync<TableEntity>(
                It.Is<string>(f => f == $"PartitionKey eq '{partitionKey}' and Date ge datetime'{fromDate:yyyy-MM-ddTHH:mm:ss.fffZ}' and Date le datetime'{toDate:yyyy-MM-ddTHH:mm:ss.fffZ}'"),
                It.IsAny<int?>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
            .Returns(asyncPageable.Object);

        var client = CreateClient();

        // Act
        var result = (await client.QueryByDateRangeAsync(fromDate, toDate, CancellationToken.None)).ToList();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldHaveSingleItem();
        var entity = result.First();
        entity.City.ShouldBe(tableEntity.GetString("City"));
        entity.Country.ShouldBe(tableEntity.GetString("Country"));
        entity.Status.ShouldBe(tableEntity.GetString("Status"));
        entity.Date.ShouldBe(tableEntity.GetDateTimeOffset("Date")!.Value);
        entity.PayloadBlobReference.ShouldBe(tableEntity.GetString("PayloadBlobReference"));

        _tableClientMock.Verify(
            x => x.QueryAsync<TableEntity>(
                It.Is<string>(f => f == $"PartitionKey eq '{partitionKey}' and Date ge datetime'{fromDate:yyyy-MM-ddTHH:mm:ss.fffZ}' and Date le datetime'{toDate:yyyy-MM-ddTHH:mm:ss.fffZ}'"),
                It.IsAny<int?>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly List<T> _items;
        private int _index = -1;

        public TestAsyncEnumerator(IEnumerable<T> items)
        {
            _items = items.ToList();
        }

        public T Current => _index >= 0 && _index < _items.Count ? _items[_index] : default!;

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            _index++;
            return ValueTask.FromResult(_index < _items.Count);
        }
    }
}
