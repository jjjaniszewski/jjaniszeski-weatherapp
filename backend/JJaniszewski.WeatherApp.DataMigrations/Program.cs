using System;
using System.IO;
using JJaniszewski.WeatherApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<WeatherDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("SqlServer")));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();

Console.WriteLine("Applying migrations...");
db.Database.Migrate();
Console.WriteLine("Migrations applied successfully.");
