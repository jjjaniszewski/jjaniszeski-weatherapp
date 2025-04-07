using Azure.Data.Tables;
using Azure.Storage.Blobs;
using JJaniszewski.WeatherApp.Application;
using JJaniszewski.WeatherApp.Application.Services;
using JJaniszewski.WeatherApp.Domain.Repositories;
using JJaniszewski.WeatherApp.Infrastructure.Data;
using JJaniszewski.WeatherApp.Infrastructure.Repositories;
using JJaniszewski.WeatherApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JJaniszewski.WeatherApp.Infrastructure;

public static class ServiceCollectionExtensions
{
    private const string SQL_SERVER_CONNECTION_STRING_NAME = "SqlServer";
    private const string AZURE_STORAGE_CONNECTION_STRING_NAME = "AzureWebJobsStorage";

    public static IServiceCollection RegisterWeatherInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterWeatherApplication(configuration);

        services.AddHttpClient<IWeatherApiClient, WeatherApiClient>();

        var connectionString = configuration.GetConnectionString(SQL_SERVER_CONNECTION_STRING_NAME);
        services.AddDbContext<WeatherDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IWeatherRecordRepository, WeatherRecordRepository>();

        services.AddSingleton<IWeatherRecordPayloadsBlobClient, AzureWeatherRecordPayloadsBlobClient>();
        services.AddSingleton<IWeatherLoadLogsTableClient, AzureWeatherLoadLogsTableClient>();

        services.AddSingleton(_ =>
        {
            var client = new BlobContainerClient(configuration.GetValue<string>(AZURE_STORAGE_CONNECTION_STRING_NAME), AzureWeatherRecordPayloadsBlobClient.CONTAINER_NAME);
            client.CreateIfNotExists();
            return client;
        });

        services.AddSingleton(_ =>
        {
            var client = new TableClient(configuration.GetValue<string>(AZURE_STORAGE_CONNECTION_STRING_NAME), AzureWeatherLoadLogsTableClient.TABLE_NAME);
            client.CreateIfNotExists();
            return client;
        });

        return services;
    }
}
