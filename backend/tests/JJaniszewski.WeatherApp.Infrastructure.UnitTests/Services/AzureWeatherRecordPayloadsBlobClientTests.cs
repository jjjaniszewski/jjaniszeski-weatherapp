using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using JJaniszewski.WeatherApp.Infrastructure.Services;
using Moq;
using Shouldly;
using Xunit;

namespace JJaniszewski.WeatherApp.Infrastructure.UnitTests.Services;

public class AzureWeatherRecordPayloadsBlobClientTests
{
    private readonly Mock<BlobContainerClient> _containerClientMock;

    public AzureWeatherRecordPayloadsBlobClientTests()
    {
        _containerClientMock = new Mock<BlobContainerClient>();
    }

    private AzureWeatherRecordPayloadsBlobClient CreateClient()
    {
        return new AzureWeatherRecordPayloadsBlobClient(_containerClientMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_UploadsContentToBlob()
    {
        // Arrange
        var blobName = "test-blob";
        var content = "test content";
        var blobClientMock = new Mock<BlobClient>();
        Stream capturedStream = null;

        _containerClientMock
            .Setup(x => x.GetBlobClient(blobName))
            .Returns(blobClientMock.Object);

        blobClientMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()))
            .Callback<Stream, bool, CancellationToken>((stream, overwrite, token) =>
            {
                var buffer = new byte[1024];
                var length = stream.Read(buffer, 0, buffer.Length);
                capturedStream = new MemoryStream(buffer, 0, length);
            })
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        var client = CreateClient();

        // Act
        await client.CreateAsync(blobName, content, CancellationToken.None);

        // Assert
        _containerClientMock.Verify(x => x.GetBlobClient(blobName), Times.Once);
        blobClientMock.Verify(
            x => x.UploadAsync(It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()),
            Times.Once);

        capturedStream.ShouldNotBeNull();
        using var reader = new StreamReader(capturedStream);
        reader.ReadToEnd().ShouldBe(content);
    }

    [Fact]
    public async Task GetAsync_WhenBlobExists_ReturnsContent()
    {
        // Arrange
        var blobName = "test-blob";
        var expectedContent = "test content";
        var blobClientMock = new Mock<BlobClient>();
        var downloadResult = new Mock<Response<BlobDownloadResult>>();
        var binaryData = BinaryData.FromString(expectedContent);
        var blobDownloadResult = BlobsModelFactory.BlobDownloadResult(binaryData);

        downloadResult.Setup(x => x.Value).Returns(blobDownloadResult);

        _containerClientMock
            .Setup(x => x.GetBlobClient(blobName))
            .Returns(blobClientMock.Object);

        blobClientMock
            .Setup(x => x.DownloadContentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(downloadResult.Object);

        var client = CreateClient();

        // Act
        var result = await client.GetAsync(blobName, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedContent);
        _containerClientMock.Verify(x => x.GetBlobClient(blobName), Times.Once);
        blobClientMock.Verify(x => x.DownloadContentAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenBlobDoesNotExist_ThrowsException()
    {
        // Arrange
        var blobName = "non-existent-blob";
        var blobClientMock = new Mock<BlobClient>();

        _containerClientMock
            .Setup(x => x.GetBlobClient(blobName))
            .Returns(blobClientMock.Object);

        blobClientMock
            .Setup(x => x.DownloadContentAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RequestFailedException(404, "Blob not found"));

        var client = CreateClient();

        // Act & Assert
        await Should.ThrowAsync<RequestFailedException>(() =>
            client.GetAsync(blobName, CancellationToken.None));

        _containerClientMock.Verify(x => x.GetBlobClient(blobName), Times.Once);
        blobClientMock.Verify(x => x.DownloadContentAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
