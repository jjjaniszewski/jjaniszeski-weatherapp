using JJaniszewski.WeatherApp.Application.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JJaniszewski.WeatherApp.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterWeatherApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging();
        services.Configure<WeatherApiOptions>(configuration.GetSection("WeatherApi"));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        return services;
    }
}
